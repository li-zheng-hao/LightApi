//参考
//zap集成 https://opentelemetry.io/docs/languages/go/instrumentation/#log-bridge
//echo集成 https://github.com/open-telemetry/opentelemetry-go-contrib/tree/main/instrumentation/github.com/labstack/echo/otelecho

package opentelemetry

import (
	"context"
	"errors"
	"go.opentelemetry.io/contrib/bridges/otelzap"
	"go.opentelemetry.io/otel/exporters/otlp/otlplog/otlploghttp"
	"go.opentelemetry.io/otel/exporters/otlp/otlptrace/otlptracehttp"
	"go.opentelemetry.io/otel/sdk/resource"
	semconv "go.opentelemetry.io/otel/semconv/v1.26.0"
	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
	"os"
	"time"

	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/log/global"
	"go.opentelemetry.io/otel/propagation"
	"go.opentelemetry.io/otel/sdk/log"
	"go.opentelemetry.io/otel/sdk/trace"
)

// SetupOTelSDK bootstraps the OpenTelemetry pipeline.
// If it does not return an error, make sure to call shutdown for proper cleanup.
func SetupOTelSDK(ctx context.Context) (logger *zap.Logger, shutdown func(context.Context) error, err error) {
	var shutdownFuncs []func(context.Context) error

	// shutdown calls cleanup functions registered via shutdownFuncs.
	// The errors from the calls are joined.
	// Each registered cleanup will be invoked once.
	shutdown = func(ctx context.Context) error {
		var err error
		for _, fn := range shutdownFuncs {
			err = errors.Join(err, fn(ctx))
		}
		shutdownFuncs = nil
		return err
	}

	// handleErr calls shutdown for cleanup and makes sure that all errors are returned.
	handleErr := func(inErr error) {
		err = errors.Join(inErr, shutdown(ctx))
	}

	// Set up propagator.
	prop := newPropagator()
	otel.SetTextMapPropagator(prop)

	// Set up trace provider.
	tracerProvider, err := newTraceProvider()
	if err != nil {
		handleErr(err)
		return
	}
	shutdownFuncs = append(shutdownFuncs, tracerProvider.Shutdown)
	otel.SetTracerProvider(tracerProvider)

	// Set up meter provider.
	//meterProvider, err := newMeterProvider()
	//if err != nil {
	//	handleErr(err)
	//	return
	//}
	//shutdownFuncs = append(shutdownFuncs, meterProvider.Shutdown)
	//opentelemetry.SetMeterProvider(meterProvider)

	// Set up logger provider.
	loggerProvider, err := newLoggerProvider()
	if err != nil {
		handleErr(err)
		return
	}

	shutdownFuncs = append(shutdownFuncs, loggerProvider.Shutdown)
	global.SetLoggerProvider(loggerProvider)

	core := zapcore.NewTee(
		zapcore.NewCore(zapcore.NewJSONEncoder(zap.NewDevelopmentEncoderConfig()), zapcore.AddSync(os.Stdout), zapcore.InfoLevel),
		otelzap.NewCore("servicename", otelzap.WithLoggerProvider(loggerProvider)),
	)
	//GlobalLog = otelzap.NewCore("my/pkg/name", otelzap.WithLoggerProvider(loggerProvider))
	logger = zap.New(core)
	return
}

func newPropagator() propagation.TextMapPropagator {
	return propagation.NewCompositeTextMapPropagator(
		propagation.TraceContext{},
		propagation.Baggage{},
	)
}

func newTraceProvider() (*trace.TracerProvider, error) {
	//traceExporter, err := stdouttrace.New(
	//	stdouttrace.WithPrettyPrint())
	res, err := newResource()
	if err != nil {
		return nil, err
	}
	ctx := context.Background()
	traceExporter, err := newExporter(ctx)
	if err != nil {
		return nil, err
	}

	traceProvider := trace.NewTracerProvider(
		trace.WithResource(res),
		trace.WithBatcher(traceExporter,
			// Default is 5s. Set to 1s for demonstrative purposes.
			trace.WithBatchTimeout(time.Second)),
	)
	return traceProvider, nil
}

//	func newMeterProvider() (*metric.MeterProvider, error) {
//		metricExporter, err := stdoutmetric.New()
//		if err != nil {
//			return nil, err
//		}
//
//		meterProvider := metric.NewMeterProvider(
//			metric.WithReader(metric.NewPeriodicReader(metricExporter,
//				// Default is 1m. Set to 3s for demonstrative purposes.
//				metric.WithInterval(3*time.Second))),
//		)
//		return meterProvider, nil
//	}
func newResource() (*resource.Resource, error) {
	return resource.Merge(resource.Default(),
		resource.NewWithAttributes(semconv.SchemaURL,
			semconv.ServiceName("go_service_b"),
		))
}

func newLoggerProvider() (*log.LoggerProvider, error) {
	res, err := newResource()
	if err != nil {
		return nil, err
	}
	exporter, err := otlploghttp.New(context.Background(), otlploghttp.WithEndpointURL("http://172.10.2.200:53410/ingest/otlp/v1/logs"),
		otlploghttp.WithInsecure())

	if err != nil {
		return nil, err
	}

	processor := log.NewBatchProcessor(exporter)
	loggerProvider := log.NewLoggerProvider(
		log.WithResource(res),
		log.WithProcessor(processor),
	)

	return loggerProvider, nil
}

func newExporter(ctx context.Context) (trace.SpanExporter, error) {
	return otlptracehttp.New(ctx, otlptracehttp.WithEndpointURL("http://172.10.2.200:53410/ingest/otlp/v1/traces"),
		otlptracehttp.WithInsecure())
}
