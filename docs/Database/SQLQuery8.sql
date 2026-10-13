-- =============================================
-- Create Indexes for Performance
-- =============================================
-- Indexes for Events table
CREATE INDEX IX_Events_OrganizerID ON Events(organizer_id);
CREATE INDEX IX_Events_Status ON Events(status);
CREATE INDEX IX_Events_StartDate ON Events(start_date);

-- Indexes for Categories table
CREATE INDEX IX_Categories_EventID ON Categories(event_id);

-- Indexes for Enrolments table
CREATE INDEX IX_Enrolments_UserID ON Enrolments(user_id);
CREATE INDEX IX_Enrolments_EventID ON Enrolments(event_id);
CREATE INDEX IX_Enrolments_CategoryID ON Enrolments(category_id);
CREATE INDEX IX_Enrolments_Status ON Enrolments(status);

-- Indexes for Results table
CREATE INDEX IX_Results_EnrolID ON Results(enrol_id);
CREATE INDEX IX_Results_CategoryID ON Results(category_id);

-- Indexes for Payments table
CREATE INDEX IX_Payments_EnrolID ON Payments(enrol_id);
CREATE INDEX IX_Payments_Status ON Payments(status);

-- Indexes for EventRoutes table
CREATE INDEX IX_EventRoutes_EventID ON EventRoutes(event_id);

-- Indexes for AuditLogs table
CREATE INDEX IX_AuditLogs_UserID ON AuditLogs(user_id);
CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(created_at);