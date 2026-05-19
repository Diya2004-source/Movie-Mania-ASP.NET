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
    'Test@123', 
    'user', 
    1, 
    GETDATE(),
    '',
    'TEST' + CONVERT(VARCHAR, CAST(RAND() * 10000 AS INT)),
    0,
    0
);