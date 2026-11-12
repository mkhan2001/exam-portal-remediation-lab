namespace ExamPortal.Api.Data;

public static class Schema
{
    // Seed passwords: alice / Passw0rd!   bob / Winter2024   admin / admin123
    public const string CreateAndSeed = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY,
    Username TEXT NOT NULL,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Candidates (
    Id INTEGER PRIMARY KEY,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL,
    Notes TEXT NULL,
    OwnerUserId INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS Registrations (
    Id INTEGER PRIMARY KEY,
    UserId INTEGER NOT NULL,
    CandidateId INTEGER NOT NULL,
    ExamCode TEXT NOT NULL,
    TestCenter TEXT NOT NULL,
    Seats INTEGER NOT NULL,
    ScheduledOn TEXT NOT NULL
);

DELETE FROM Users;
DELETE FROM Candidates;
DELETE FROM Registrations;

INSERT INTO Users (Id, Username, PasswordHash, Role) VALUES
    (1, 'alice', 'f4a69973e7b0bf9d160f9f60e3c3acd2494beb0d', 'Scheduler'),
    (2, 'bob',   '0405f09e8ccd8ce4236bdb6b167e4426bfc41848',   'Scheduler'),
    (3, 'admin', 'f865b53623b121fd34ee5426c792e5c33af8c227', 'Admin');

INSERT INTO Candidates (Id, FirstName, LastName, Email, Notes, OwnerUserId) VALUES
    (1, 'Maria',  'Alvarez',  'maria.alvarez@example.com',  'Needs extended time.', 1),
    (2, 'Devon',  'Brooks',   'devon.brooks@example.com',   NULL, 1),
    (3, 'Priya',  'Chandra',  'priya.chandra@example.com',  'Rescheduled twice.', 2),
    (4, 'Samuel', 'O''Brien', 'samuel.obrien@example.com',  NULL, 2);

INSERT INTO Registrations (Id, UserId, CandidateId, ExamCode, TestCenter, Seats, ScheduledOn) VALUES
    (1, 1, 1, 'BAR-CA-2026', 'Oakland',   1, '2026-07-28'),
    (2, 1, 2, 'BAR-CA-2026', 'San Diego', 1, '2026-07-28'),
    (3, 2, 3, 'CPA-AUD',     'Chicago',   1, '2026-08-14');
";
}
