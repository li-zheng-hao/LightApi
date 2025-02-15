package migrations

import (
	"embed"
	"go-template/internal/core"
	"log"

	_ "github.com/golang-migrate/migrate/v4/database/sqlserver"

	"github.com/golang-migrate/migrate/v4"
	"github.com/golang-migrate/migrate/v4/source/iofs"
)

//go:embed scripts/*
var dbMigrations embed.FS

func MigrateDatabase() {
	core.App.Logger.Debug("开始迁移数据库...")
	d, err := iofs.New(dbMigrations, "scripts")
	if err != nil {
		log.Fatal(err)
	}
	m, err := migrate.NewWithSourceInstance("iofs", d, core.App.Config.SqlServerUrl)
	if err != nil {
		log.Fatal(err)
	}
	err = m.Up()
	if err != nil {
		// ...
	}
	// ...
	core.App.Logger.Debug("迁移数据库成功")
}
