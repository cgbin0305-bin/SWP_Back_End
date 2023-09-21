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

BEGIN TRANSACTION;

ALTER TABLE "Worker" ADD "Password" TEXT NULL;

CREATE TABLE "ef_temp_Worker" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Worker" PRIMARY KEY AUTOINCREMENT,
    "Email" TEXT NOT NULL,
    "Fee" money NOT NULL,
    "IsAdmin" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Password" TEXT NULL,
    "Phone" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "Version" INTEGER NOT NULL
);

INSERT INTO "ef_temp_Worker" ("Id", "Email", "Fee", "IsAdmin", "Name", "Password", "Phone", "Status", "Version")
SELECT "Id", "Email", "Fee", "IsAdmin", "Name", "Password", "Phone", "Status", "Version"
FROM "Worker";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Worker";

ALTER TABLE "ef_temp_Worker" RENAME TO "Worker";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230916113509_changePasswordColumn', '7.0.10');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "HouseHoldChores" ADD "Description" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230917093053_AddDescriptionCol', '7.0.10');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "OrderHistory" RENAME COLUMN "UserName" TO "GuestName";

ALTER TABLE "HouseHoldChores" RENAME COLUMN "ChoresName" TO "Version";

ALTER TABLE "Workers_Chores" ADD "Version" TEXT NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "HouseHoldChores" ADD "Name" TEXT NOT NULL DEFAULT '';

CREATE TABLE "Reviews" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Reviews" PRIMARY KEY,
    "Content" TEXT NULL,
    "Date" TEXT NOT NULL,
    "Rate" INTEGER NOT NULL,
    "Version" TEXT NOT NULL,
    CONSTRAINT "FK_Reviews_OrderHistory_Id" FOREIGN KEY ("Id") REFERENCES "OrderHistory" ("Id")
);

CREATE TABLE "User" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_User" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "PasswordHash" BLOB NULL,
    "PasswordSalt" BLOB NULL,
    "Phone" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Role" TEXT NULL,
    "Version" TEXT NOT NULL
);

CREATE TABLE "ef_temp_Worker" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Worker" PRIMARY KEY,
    "Fee" money NOT NULL,
    "Status" INTEGER NOT NULL,
    "Version" TEXT NOT NULL,
    CONSTRAINT "FK_Worker_User_Id" FOREIGN KEY ("Id") REFERENCES "User" ("Id")
);

INSERT INTO "ef_temp_Worker" ("Id", "Fee", "Status", "Version")
SELECT "Id", "Fee", "Status", "Version"
FROM "Worker";

CREATE TABLE "ef_temp_OrderHistory" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderHistory" PRIMARY KEY AUTOINCREMENT,
    "Date" TEXT NOT NULL,
    "GuestAddress" TEXT NOT NULL,
    "GuestEmail" TEXT NOT NULL,
    "GuestName" TEXT NOT NULL,
    "GuestPhone" TEXT NOT NULL,
    "Version" TEXT NOT NULL,
    "WorkerId" INTEGER NOT NULL,
    CONSTRAINT "FK_OrderHistory_Worker_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Worker" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_OrderHistory" ("Id", "Date", "GuestAddress", "GuestEmail", "GuestName", "GuestPhone", "Version", "WorkerId")
SELECT "Id", "Date", "GuestAddress", "GuestEmail", "GuestName", "GuestPhone", "Version", "WorkerId"
FROM "OrderHistory";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Worker";

ALTER TABLE "ef_temp_Worker" RENAME TO "Worker";

DROP TABLE "OrderHistory";

ALTER TABLE "ef_temp_OrderHistory" RENAME TO "OrderHistory";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_OrderHistory_WorkerId" ON "OrderHistory" ("WorkerId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230920124832_Update_DB', '7.0.10');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "User" ADD "Address" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230920133722_Add_Address_Column_User_Table', '7.0.10');

COMMIT;

