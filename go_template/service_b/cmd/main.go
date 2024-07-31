package main

import (
	"context"
	"errors"
	"fmt"
	"go.opentelemetry.io/otel"
	"go.uber.org/zap"
	"os"
	"os/signal"
	"pkg/opentelemetry"
	"time"
)

func main() {
	if err := run(); err != nil {
		fmt.Println(err)
	}
}

func run() (err error) {
	// Handle SIGINT (CTRL+C) gracefully.
	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt)
	defer stop()

	// Set up OpenTelemetry.
	otelShutdown, err := opentelemetry.SetupOTelSDK(ctx)
	if err != nil {
		return
	}

	// Handle shutdown properly so nothing leaks.
	defer func() {
		err = errors.Join(err, otelShutdown(context.Background()))
	}()
	Test()
	time.Sleep(time.Second * 5)
	return
}

func Test() {
	traceProvider := otel.GetTracerProvider()
	tracer := traceProvider.Tracer("test")
	ctx, span := tracer.Start(context.Background(), "Span名字")
	defer span.End()
	span.AddEvent("test事件")
	opentelemetry.GlobalLog.Info("测试日志", zap.Any("context", ctx))
	Test2(ctx)
}

func Test2(ctx context.Context) {
	traceProvider := otel.GetTracerProvider()
	tracer := traceProvider.Tracer("test2")
	ctx2, span := tracer.Start(ctx, "Span名字2")
	defer span.End()
	span.AddEvent("test事件2")
	opentelemetry.GlobalLog.Info("测试日志2", zap.Any("context", ctx2))
}
