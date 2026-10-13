namespace RaceDay.Contracts;

// ============================================================
// Authentication DTOs
// ============================================================
public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role);

public record LoginRequest(
    string Email,
    string Password);

public record AuthResponse(
    int UserId,
    string Email,
    string FullName,
    string Role);

// ============================================================
// Profile DTOs
// ============================================================
public record ProfileDto(
    int UserId,
    string FullName,
    string Email,
    string Role,
    string? PhoneNumber);

public record UpdateProfileRequest(
    string FullName,
    string? PhoneNumber);

// ============================================================
// Event DTOs
// ============================================================
public record CreateEventRequest(
    string Title,
    string? Description,        // ← nullable
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    decimal DistanceKm,
    string EventType);

public record UpdateEventRequest(
    string Title,
    string? Description,        // ← nullable
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    decimal DistanceKm,
    string EventType,
    string Status);

public record EventDto(
    int EventId,
    string Title,
    string? Description,        // ← nullable
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    decimal DistanceKm,
    string EventType,
    string Status,
    int OrganizerId);

// ============================================================
// Category DTOs
// ============================================================
public record CreateCategoryRequest(
    string Name,
    string? Description,
    decimal DistanceKm,
    decimal EntryFee,
    TimeOnly StartTime,
    int? MaxParticipants);

public record CategoryDto(
    int CategoryId,
    int EventId,
    string Name,
    string? Description,
    decimal DistanceKm,
    decimal EntryFee,
    TimeOnly StartTime,
    int? MaxParticipants);

// ============================================================
// Enrolment DTOs
// ============================================================
public record CreateEnrolmentRequest(
    int EventId,
    int CategoryId);

public record UpdateEnrolmentStatusRequest(
    string Status,
    string? BibNumber);

public record EnrolmentDto(
    int EnrolId,
    int UserId,
    int EventId,
    int CategoryId,
    string Status,
    string? BibNumber,
    DateTime EntryDate);

// ============================================================
// Result DTOs
// ============================================================
public record CreateResultRequest(
    int EnrolId,
    string FinishTime,
    int Position,
    string? AgeCategory);

public record ResultDto(
    int ResultId,
    int EnrolId,
    string FinishTime,
    int Position,
    string? AgeCategory,
    bool IsVerified,
    DateTime CapturedAt);