using EODService.Models;
using EODService.Models.Provider;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace EODService.Persistance
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext dbContext)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            try
            {
                // ─────────────────────────────────────────────
                // Create required tables if they do not exist
                // ─────────────────────────────────────────────

                if (!await TableExistsAsync(connection, "PROVIDER"))
                    await CreateProviderTableAsync(connection);

                if (!await TableExistsAsync(connection, "EOD_STOCKS"))
                    await CreateStockTableAsync(connection);

                if (!await TableExistsAsync(connection, "EodDaily"))
                    await CreateEodDailyTableAsync(connection);

                if (!await TableExistsAsync(connection, "EodHistory"))
                    await CreateEodHistoryTableAsync(connection);

                if (!await SequenceExistsAsync(connection, "SEQ_EOD_STOCKS_ID"))
                    await CreateStockSequenceAsync(connection);

                if (!await TriggerExistsAsync(connection, "TRG_EOD_STOCKS_ID"))
                    await CreateStockTriggerAsync(connection);

                // ─────────────────────────────────────────────
                // Seed providers
                // ─────────────────────────────────────────────

                await SeedProvidersAsync(connection);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        // ─────────────────────────────────────────────────────
        // Check table existence
        // ─────────────────────────────────────────────────────

        private static async Task<bool> TableExistsAsync(
            DbConnection connection,
            string tableName)
        {
            using var command = connection.CreateCommand();

            command.CommandText = $@"
                SELECT COUNT(*)
                FROM USER_TABLES
                WHERE TABLE_NAME = '{tableName.ToUpperInvariant()}'";

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }


        // ─────────────────────────────────────────────────────
        // Check trigger existence
        // ─────────────────────────────────────────────────────

        private static async Task<bool> TriggerExistsAsync(
    DbConnection connection,
    string triggerName)
        {
            using var command = connection.CreateCommand();

            command.CommandText = $@"
        SELECT COUNT(*)
        FROM USER_TRIGGERS
        WHERE TRIGGER_NAME = '{triggerName.ToUpperInvariant()}'";

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // ─────────────────────────────────────────────────────
        // Check sequence existence
        // ─────────────────────────────────────────────────────


        private static async Task<bool> SequenceExistsAsync(
    DbConnection connection,
    string sequenceName)
        {
            using var command = connection.CreateCommand();

            command.CommandText = $@"
        SELECT COUNT(*)
        FROM USER_SEQUENCES
        WHERE SEQUENCE_NAME = '{sequenceName.ToUpperInvariant()}'";

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // ─────────────────────────────────────────────────────
        // Create PROVIDER
        // ─────────────────────────────────────────────────────

        private static async Task CreateProviderTableAsync(
            DbConnection connection)
        {
            await ExecuteSqlAsync(connection, @"
                CREATE TABLE PROVIDER
                (
                    ID          NUMBER(10)       NOT NULL,
                    PROVIDER    VARCHAR2(100)    NOT NULL,
                    API_KEY     VARCHAR2(500),
                    BASE_URL    VARCHAR2(500)    NOT NULL,
                    ENDPOINT    VARCHAR2(500)    NOT NULL,
                    PARAMETERS  VARCHAR2(4000)   NOT NULL,

                    CONSTRAINT PK_PROVIDER
                        PRIMARY KEY (ID)
                )");
        }

        // ─────────────────────────────────────────────────────
        // Create EOD_STOCKS
        // ─────────────────────────────────────────────────────

        private static async Task CreateStockTableAsync(
            DbConnection connection)
        {
            await ExecuteSqlAsync(connection, @"
                CREATE TABLE EOD_STOCKS
                (
                    STOCK_ID      NUMBER(10)       NOT NULL,
                    SC_COMP_ID    NUMBER(4),
                    ISIN          VARCHAR2(50)     NOT NULL,
                    STOCK_NAME    VARCHAR2(200)    NOT NULL,
                    Y_FINANCE     VARCHAR2(100),
                    T_DATA        VARCHAR2(100),
                    LSEG          VARCHAR2(100),
                    YF_FLAG       VARCHAR2(1)      NOT NULL,
                    LSEG_FLAG     VARCHAR2(1)      NOT NULL,
                    TD_FLAG       VARCHAR2(1)      NOT NULL,
                    SC_EXCHANGE   VARCHAR2(50),

                    CONSTRAINT PK_EOD_STOCKS
                        PRIMARY KEY (STOCK_ID),

                    CONSTRAINT CK_EOD_STOCKS_YF_FLAG
                        CHECK (YF_FLAG IN ('Y', 'N')),

                    CONSTRAINT CK_EOD_STOCKS_LSEG_FLAG
                        CHECK (LSEG_FLAG IN ('Y', 'N')),

                    CONSTRAINT CK_EOD_STOCKS_TD_FLAG
                        CHECK (TD_FLAG IN ('Y', 'N'))
                )");
        }

        // ─────────────────────────────────────────────────────
        // Create EODDAILY
        // ─────────────────────────────────────────────────────

        private static async Task CreateEodDailyTableAsync(
            DbConnection connection)
        {
            await ExecuteSqlAsync(connection, @"
                CREATE TABLE EODDAILY
                (
                    STOCK_ID       NUMBER(10)      NOT NULL,
                    STOCK_NAME     VARCHAR2(200),
                    ""DATE""       DATE,
                    OPEN           NUMBER(18,4),
                    HIGH           NUMBER(18,4),
                    LOW            NUMBER(18,4),
                    CLOSE          NUMBER(18,4),
                    ADJUSTEDCLOSE  NUMBER(18,4),
                    VOLUME         NUMBER,

                    CONSTRAINT PK_EODDAILY
                        PRIMARY KEY (STOCK_ID)
                )");
        }

        // ─────────────────────────────────────────────────────
        // Create EODHISTORY
        // ─────────────────────────────────────────────────────

        private static async Task CreateEodHistoryTableAsync(
            DbConnection connection)
        {
            await ExecuteSqlAsync(connection, @"
                CREATE TABLE EODHISTORY
                (
                    STOCK_ID       NUMBER(10)      NOT NULL,
                    STOCK_NAME     VARCHAR2(200),
                    ""DATE""       DATE            NOT NULL,
                    OPEN           NUMBER(18,4),
                    HIGH           NUMBER(18,4),
                    LOW            NUMBER(18,4),
                    CLOSE          NUMBER(18,4),
                    ADJUSTEDCLOSE  NUMBER(18,4),
                    VOLUME         NUMBER,

                    CONSTRAINT PK_EODHISTORY
                        PRIMARY KEY (STOCK_ID, ""DATE"")
                )");
        }

        // ─────────────────────────────────────────────────────
        // Execute SQL
        // ─────────────────────────────────────────────────────

        private static async Task ExecuteSqlAsync(
            DbConnection connection,
            string sql)
        {
            using var command = connection.CreateCommand();

            command.CommandText = sql;

            await command.ExecuteNonQueryAsync();
        }

        private static async Task CreateStockSequenceAsync(
    DbConnection connection)
        {
            await ExecuteSqlAsync(connection, @"
        CREATE SEQUENCE SEQ_EOD_STOCKS_ID
        START WITH 1
        INCREMENT BY 1
        NOCACHE
        NOCYCLE");
        }
private static async Task CreateStockTriggerAsync(
    DbConnection connection)
        {
            await ExecuteSqlAsync(connection, @"
        CREATE OR REPLACE TRIGGER TRG_EOD_STOCKS_ID
        BEFORE INSERT ON EOD_STOCKS
        FOR EACH ROW
        BEGIN
            IF :NEW.STOCK_ID IS NULL THEN
                SELECT SEQ_EOD_STOCKS_ID.NEXTVAL
                INTO :NEW.STOCK_ID
                FROM DUAL;
            END IF;
        END;");
        }

        // ─────────────────────────────────────────────────────
        // Seed Providers
        // ─────────────────────────────────────────────────────

        private static async Task SeedProvidersAsync(
            DbConnection connection)
        {
            // Yahoo
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT COUNT(*) FROM PROVIDER WHERE ID = 1";

                var count = Convert.ToInt32(
                    await command.ExecuteScalarAsync());

                if (count == 0)
                {
                    command.CommandText = @"
                        INSERT INTO PROVIDER
                        (
                            ID,
                            PROVIDER,
                            API_KEY,
                            BASE_URL,
                            ENDPOINT,
                            PARAMETERS
                        )
                        VALUES
                        (
                            1,
                            'Yahoo',
                            NULL,
                            'https://query1.finance.yahoo.com',
                            '/v8/finance/chart',
                            '{""Interval"":""1d"",""Range"":""3d""}'
                        )";

                    await command.ExecuteNonQueryAsync();
                }
            }

            // TwelveData
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT COUNT(*) FROM PROVIDER WHERE ID = 2";

                var count = Convert.ToInt32(
                    await command.ExecuteScalarAsync());

                if (count == 0)
                {
                    command.CommandText = @"
                        INSERT INTO PROVIDER
                        (
                            ID,
                            PROVIDER,
                            API_KEY,
                            BASE_URL,
                            ENDPOINT,
                            PARAMETERS
                        )
                        VALUES
                        (
                            2,
                            'TwelveData',
                            '21c6115364964470ac04579771a25555',
                            'https://api.twelvedata.com',
                            '/time_series',
                            '{""Interval"":""1day"",""OutputSize"":5}'
                        )";

                    await command.ExecuteNonQueryAsync();
                }
            }

            // LSEG
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT COUNT(*) FROM PROVIDER WHERE ID = 3";

                var count = Convert.ToInt32(
                    await command.ExecuteScalarAsync());

                if (count == 0)
                {
                    command.CommandText = @"
                        INSERT INTO PROVIDER
                        (
                            ID,
                            PROVIDER,
                            API_KEY,
                            BASE_URL,
                            ENDPOINT,
                            PARAMETERS
                        )
                        VALUES
                        (
                            3,
                            'LSEG',
                            '11349f3a5b504e98bd5e85b6620fe21f4acb21b1',
                            'ws://10.110.221.99:15000',
                            '/WebSocket',
                            '{""DacsUser"":""EODService"",""ServiceId"":27,""ServiceName"":""ELEKTRON_DD"",""ApplicationId"":""256""}'
                        )";

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
