package db

import (
	"gorm.io/driver/sqlserver"
	"gorm.io/gorm"
	"time"
)

func NewDbConnection(dbConnString string) (*gorm.DB, error) {
	db, err := gorm.Open(sqlserver.Open(dbConnString), &gorm.Config{})
	if err != nil {
		return nil, err
	}

	sqlDB, err := db.DB()
	if err != nil {
		return nil, err
	}
	sqlDB.SetMaxIdleConns(1)
	sqlDB.SetMaxOpenConns(100)
	sqlDB.SetConnMaxLifetime(time.Minute * 30)
	return db, nil
}
