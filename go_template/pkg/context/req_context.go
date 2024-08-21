package context

import (
	"context"
	"github.com/labstack/echo/v4"
	"go.opentelemetry.io/otel/trace"
	"go.uber.org/zap"
	"pkg/global"
)

type RequestContext struct {
	echo.Context
	ContextLogger *zap.Logger
	UserContext   *UserContext
	span          *trace.Span
	spanContext   *context.Context
}

func NewContext(ctx echo.Context, logger *zap.Logger, userContext *UserContext) *RequestContext {
	newContext := &RequestContext{ctx, logger, userContext, nil, nil}
	return newContext
}

func (receiver *RequestContext) IsLogin() bool {
	if receiver.UserContext == nil {
		return false
	}
	return receiver.UserContext.IsLogin()
}

func (receiver *RequestContext) StartWithNewTracer(spanName string) *trace.Span {
	if receiver.spanContext == nil {
		ctx, span := global.App.Tracer.Start(receiver.Context.Request().Context(), spanName)
		receiver.span = &span
		receiver.spanContext = &ctx
	} else {
		ctx, span := global.App.Tracer.Start(*receiver.spanContext, spanName)
		receiver.span = &span
		receiver.spanContext = &ctx
	}

	userName := receiver.UserContext.Name
	if len(userName) <= 0 {
		userName = "未登录"
	}
	receiver.ContextLogger = receiver.ContextLogger.With(zap.Any("context", *receiver.spanContext),
		zap.String("user", userName),
		zap.String("userid", string(receiver.UserContext.Id)))

	return receiver.span
}
