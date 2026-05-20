INSERT INTO Users (
    Name, 
    Email, 
    Password, 
    Role, 
    IsActive, 
    CreatedAt, 
    ProfilePictureUrl,
    ReferralCode,
    TotalReferrals,
    RewardPoints
)
VALUES (
    'Test User', 
    'test@example.com', 
    'Test@123',  -- This is the plain text password
    'user', 
    1, 
    GETDATE(),
    '',
    'TEST123',
    0,
    0
);