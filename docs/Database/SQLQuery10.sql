-- =============================================
-- Verification Queries
-- =============================================
USE RaceDayDB;

-- Show all users
SELECT user_id, email, full_name, role, is_active FROM Users;

-- Show all events with organizer info
SELECT e.event_id, e.title, u.full_name as organizer, e.start_date, e.location, e.status
FROM Events e
JOIN Users u ON e.organizer_id = u.user_id;

-- Show categories per event
SELECT ev.title as event_name, c.name as category_name, c.distance_km, c.entry_fee
FROM Categories c
JOIN Events ev ON c.event_id = ev.event_id
ORDER BY ev.title, c.distance_km;

-- Show enrolment statistics
SELECT ev.title, COUNT(e.enrol_id) as total_enrolments,
       SUM(CASE WHEN e.status = 'Confirmed' THEN 1 ELSE 0 END) as confirmed,
       SUM(CASE WHEN e.status = 'Pending' THEN 1 ELSE 0 END) as pending
FROM Enrolments e
JOIN Events ev ON e.event_id = ev.event_id
GROUP BY ev.title;

-- Show results with participant names
SELECT r.result_id, u.full_name as participant, ev.title as event, c.name as category,
       r.finish_time, r.position, r.is_verified
FROM Results r
JOIN Enrolments e ON r.enrol_id = e.enrol_id
JOIN Users u ON e.user_id = u.user_id
JOIN Events ev ON e.event_id = ev.event_id
JOIN Categories c ON r.category_id = c.category_id
ORDER BY ev.title, r.position;
