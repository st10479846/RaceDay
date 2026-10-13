-- =============================================
-- 5. Enrolments Table
-- =============================================
CREATE TABLE dbo.Enrolments (
    enrol_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    event_id INT NOT NULL,
    category_id INT NOT NULL,
    status VARCHAR(20) NOT NULL CONSTRAINT CK_Enrolments_Status CHECK (status IN ('Pending','Confirmed','CheckedIn','Completed','Withdrawn')) DEFAULT 'Pending',
    entry_date DATETIME NOT NULL DEFAULT GETDATE(),
    start_time TIME NULL,
    finish_time TIME NULL,
    bib_number VARCHAR(20) NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    waiver_signed BIT NOT NULL DEFAULT 0,
    emergency_contact VARCHAR(100) NULL,
    emergency_phone VARCHAR(20) NULL,
    CONSTRAINT FK_Enrolments_Users FOREIGN KEY (user_id) REFERENCES dbo.Users(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (event_id) REFERENCES dbo.Events(event_id) ON DELETE NO ACTION,
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (category_id) REFERENCES dbo.Categories(category_id) ON DELETE NO ACTION,
    CONSTRAINT UQ_Enrolment_User_Event UNIQUE (user_id, event_id)
);
GO

CREATE TRIGGER dbo.trg_Enrolments_UpdateTimestamp
ON dbo.Enrolments
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Avoid recursion when we set updated_at below
    IF UPDATE(updated_at)
        RETURN;

    UPDATE e
    SET updated_at = GETDATE()
    FROM dbo.Enrolments e
    INNER JOIN inserted i ON e.enrol_id = i.enrol_id;
END;
GO