package internal_log

import (
	"github.com/labstack/gommon/log"
	"github.com/li-zheng-hao/seqgo"
	logrus "github.com/sirupsen/logrus"
	"io"
	"os"
)

type SysLog struct {
	log     *logrus.Logger
	prefix  string
	seqHook *seqgo.SeqHook
}

var SysLogger = newSysLog()

func newSysLog() *SysLog {
	logger := &SysLog{}
	logger.log = logrus.New()
	logger.log.Out = os.Stdout
	//logger.log.SetOutput(&lumberjack.Logger{
	//	Filename:   "system.log",
	//	MaxSize:    50, // megabytes
	//	MaxBackups: 3,
	//	MaxAge:     7,     //days
	//	Compress:   false, // disabled by default
	//})
	logger.log.Formatter = &logrus.TextFormatter{}
	logger.log.Level = logrus.DebugLevel
	hook := seqgo.NewSeqHook(func(options *seqgo.SeqHookOptions) {
		options.BatchSize = 10
		options.Fields = map[string]string{
			"System": "go_template",
			"Env":    "Development",
		}
		options.Endpoint = "http://172.10.2.200:53410"
	})
	logger.log.AddHook(hook)
	// 新增控制台hook

	logger.seqHook = hook
	return logger
}
func (l *SysLog) Output() io.Writer {
	return l.log.Out
}

func (l *SysLog) SetOutput(w io.Writer) {
	l.log.Out = w
}

func (l *SysLog) Prefix() string {
	return l.prefix
}

func (l *SysLog) SetPrefix(p string) {
	l.prefix = p
}

func (l *SysLog) Level() log.Lvl {
	return log.Lvl(l.log.Level)
}

func (l *SysLog) SetLevel(v log.Lvl) {
	if v == log.DEBUG {
		l.log.Level = logrus.DebugLevel
	} else if v == log.INFO {
		l.log.Level = logrus.InfoLevel
	} else if v == log.WARN {
		l.log.Level = logrus.WarnLevel
	}
}

func (l *SysLog) SetHeader(h string) {
	// TODO
}

func (l *SysLog) Print(i ...interface{}) {
	l.log.Print(i...)
}

func (l *SysLog) Printf(format string, args ...interface{}) {
	l.log.Printf(format, args...)
}

func (l *SysLog) Printj(j log.JSON) {
	// TODO
	println("Printj", j)
}

func (l *SysLog) Debug(i ...interface{}) {
	l.log.Debug(i...)
}

func (l *SysLog) Debugf(format string, args ...interface{}) {
	l.log.Debugf(format, args...)
}

func (l *SysLog) Debugj(j log.JSON) {
	println("Debugj", j)

}

func (l *SysLog) Info(i ...interface{}) {
	l.log.Info(i...)
}

func (l *SysLog) Infof(format string, args ...interface{}) {
	l.log.Infof(format, args...)
}

func (l *SysLog) Infoj(j log.JSON) {
	println("Infoj", j)
}

func (l *SysLog) Warn(i ...interface{}) {
	l.log.Warn(i...)
}

func (l *SysLog) Warnf(format string, args ...interface{}) {
	l.log.Warnf(format, args...)
}

func (l *SysLog) Warnj(j log.JSON) {
	// TODO
}

func (l *SysLog) Error(i ...interface{}) {
	l.log.Error(i...)
}

func (l *SysLog) Errorf(format string, args ...interface{}) {
	l.log.Errorf(format, args...)
}

func (l *SysLog) Errorj(j log.JSON) {
	// TODO
}

func (l *SysLog) Fatal(i ...interface{}) {
	l.log.Fatal(i...)
}

func (l *SysLog) Fatalj(j log.JSON) {
	// TODO
}

func (l *SysLog) Fatalf(format string, args ...interface{}) {
	l.log.Fatalf(format, args...)
}

func (l *SysLog) Panic(i ...interface{}) {
	l.log.Panic(i...)
}

func (l *SysLog) Panicj(j log.JSON) {
	// TODO
}

func (l *SysLog) Panicf(format string, args ...interface{}) {
	l.log.Panicf(format, args...)
}

func (l *SysLog) Flush() {
	l.seqHook.Flush()
}
