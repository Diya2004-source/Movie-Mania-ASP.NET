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
    RewardPoints,
    LastLoginAt
)
VALUES (
    'Administrator', 
    'admin@moviemania.com', 
    'Admin@123', 
    'admin', 
    1, 
    GETDATE(),
    '',                          -- Empty string for ProfilePictureUrl
    'ADMIN' + CONVERT(VARCHAR, CAST(RAND() * 1000000 AS INT)),  -- Generate unique referral code
    0,                           -- TotalReferrals default
    0,                           -- RewardPoints default
    NULL                         -- LastLoginAt can be NULL
);