CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "DomesticWorker" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DomesticWorker" PRIMARY KEY AUTOINCREMENT,
    "UserName" TEXT NOT NULL,
    "Fee" money NOT NULL,
    "Phone" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "IsFree" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "Version" INTEGER NOT NULL
);

CREATE TABLE "OrderHistory" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderHistory" PRIMARY KEY AUTOINCREMENT,
    "Date" TEXT NOT NULL,
    "UserName" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "DomesticWorkerId" INTEGER NOT NULL,
    CONSTRAINT "FK_OrderHistory_DomesticWorker_DomesticWorkerId" FOREIGN KEY ("DomesticWorkerId") REFERENCES "DomesticWorker" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_OrderHistory_DomesticWorkerId" ON "OrderHistory" ("DomesticWorkerId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230916033617_InitialCreateTable', '7.0.10');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "OrderHistory" ADD "Version" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230916034420_AddVersionColIntoOrderHistoryTable', '7.0.10');

COMMIT;

BEGIN TRANSACTION;

DROP TABLE "DomesticWorker";

ALTER TABLE "OrderHistory" RENAME COLUMN "Phone" TO "GuestPhone";

ALTER TABLE "OrderHistory" RENAME COLUMN "DomesticWorkerId" TO "WorkerId";

ALTER TABLE "OrderHistory" RENAME COLUMN "Address" TO "GuestEmail";

DROP INDEX "IX_OrderHistory_DomesticWorkerId";

CREATE INDEX "IX_OrderHistory_WorkerId" ON "OrderHistory" ("WorkerId");

ALTER TABLE "OrderHistory" ADD "GuestAddress" TEXT NOT NULL DEFAULT '';

CREATE TABLE "HouseHoldChores" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HouseHoldChores" PRIMARY KEY AUTOINCREMENT,
    "ChoresName" TEXT NOT NULL
);

CREATE TABLE "Worker" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Worker" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "PasswordHash" BLOB NULL,
    "PasswordSalt" BLOB NULL,
    "Fee" money NOT NULL,
    "Phone" TEXT NULL,
    "Email" TEXT NOT NULL,
    "IsAdmin" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "Version" INTEGER NOT NULL
);

CREATE TABLE "Workers_Chores" (
    "WorkerId" INTEGER NOT NULL,
    "ChoreId" INTEGER NOT NULL,
    CONSTRAINT "PK_Workers_Chores" PRIMARY KEY ("WorkerId", "ChoreId"),
    CONSTRAINT "FK_Workers_Chores_HouseHoldChores_ChoreId" FOREIGN KEY ("ChoreId") REFERENCES "HouseHoldChores" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Workers_Chores_Worker_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Worker" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Workers_Chores_ChoreId" ON "Workers_Chores" ("ChoreId");

CREATE TABLE "ef_temp_OrderHistory" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderHistory" PRIMARY KEY AUTOINCREMENT,
    "Date" TEXT NOT NULL,
    "GuestAddress" TEXT NOT NULL,
    "GuestEmail" TEXT NOT NULL,
    "GuestPhone" TEXT NOT NULL,
    "UserName" TEXT NOT NULL,
    "Version" INTEGER NOT NULL,
    "WorkerId" INTEGER NOT NULL,
    CONSTRAINT "FK_OrderHistory_Worker_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Worker" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_OrderHistory" ("Id", "Date", "GuestAddress", "GuestEmail", "GuestPhone", "UserName", "Version", "WorkerId")
SELECT "Id", "Date", "GuestAddress", "GuestEmail", "GuestPhone", "UserName", "Version", "WorkerId"
FROM "OrderHistory";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "OrderHistory";

ALTER TABLE "ef_temp_OrderHistory" RENAME TO "OrderHistory";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_OrderHistory_WorkerId" ON "OrderHistory" ("WorkerId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230916094820_ChangeColumnDataBase_AddHousHoldChoresTable', '7.0.10');

COMMIT;

