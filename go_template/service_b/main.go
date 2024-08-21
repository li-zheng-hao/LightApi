package main

import (
	"context"
	"fmt"
	"time"
)

func main() {

	_, _ = slowOperationWithTimeout(context.Background())

	time.Sleep(50 * time.Second)
}
func slowOperationWithTimeout(ctx context.Context) (int, error) {
	ctx, cancel := context.WithTimeout(ctx, 5*time.Second)
	defer func() {
		fmt.Println("触发defer")
		cancel()

	}() // releases resources if slowOperation completes before timeout elapses
	_, _ = slowOperation(ctx)
	return 1, nil
}

func slowOperation(ctx context.Context) (int, error) {
	for {
		select {
		case <-ctx.Done():
			fmt.Println("触发取消")
			return 0, nil
		case <-time.After(1 * time.Second):
			fmt.Println("等待1秒")
			break
		}
	}

}

//
//
//import (
//	"context"
//	"errors"
//	"fmt"
//	"go.opentelemetry.io/otel"
//	"go.uber.org/zap"
//	"os"
//	"os/signal"
//	"pkg/opentelemetry"
//	"time"
//)
//
//func main() {
//	if err := run(); err != nil {
//		fmt.Println(err)
//	}
//}
//
//func run() (err error) {
//	// Handle SIGINT (CTRL+C) gracefully.
//	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt)
//	defer stop()
//
//	// Set up OpenTelemetry.
//	otelShutdown, err := opentelemetry.SetupOTelSDK(ctx)
//	if err != nil {
//		return
//	}
//
//	// Handle shutdown properly so nothing leaks.
//	defer func() {
//		err = errors.Join(err, otelShutdown(context.Background()))
//	}()
//	Test()
//	time.Sleep(time.Second * 5)
//	return
//}
//
//func Test() {
//	traceProvider := otel.GetTracerProvider()
//	tracer := traceProvider.Tracer("test")
//	ctx, span := tracer.Start(context.Background(), "Span名字")
//	defer span.End()
//	span.AddEvent("test事件")
//	opentelemetry.GlobalLog.Info("测试日志", zap.Any("context", ctx))
//	Test2(ctx)
//}
//
//func Test2(ctx context.Context) {
//	traceProvider := otel.GetTracerProvider()
//	tracer := traceProvider.Tracer("test2")
//	ctx2, span := tracer.Start(ctx, "Span名字2")
//	defer span.End()
//	span.AddEvent("test事件2")
//	opentelemetry.GlobalLog.Info("测试日志2", zap.Any("context", ctx2))
//}
