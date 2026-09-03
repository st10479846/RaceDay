-- =============================================
-- 4. Event Routes Table
-- =============================================
CREATE TABLE EventRoutes (
    route_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL,
    route_name VARCHAR(100) NOT NULL,
    start_point VARCHAR(255) NOT NULL,
    end_point VARCHAR(255) NOT NULL,
    distance_km DECIMAL(5,2) NOT NULL,
    altitude_gain INT NULL,
    gpx_track_url VARCHAR(255) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    CONSTRAINT FK_EventRoutes_Events FOREIGN KEY (event_id) REFERENCES Events(event_id) ON DELETE CASCADE
);