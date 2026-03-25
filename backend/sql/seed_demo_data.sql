
BEGIN;
DELETE FROM "Notifications";
DELETE FROM "ChatMessages";
DELETE FROM "Chats";
DELETE FROM "Appointments";
DELETE FROM "ProviderWorkingHours";
DELETE FROM "ProviderServices";
DELETE FROM "Users"
WHERE "Email" LIKE '%@bookwise.local';
INSERT INTO "Users" (
    "Email", "Username", "PasswordHash", "FirstName", "LastName", "Role", "ProfileImagePath", "Bio", "PhoneNumber",
    "CompanyName", "PrimaryCategory", "CompanyDescription", "City", "Address", "Website", "IsActive", "CreatedAt"
)
VALUES
('admin@bookwise.local', 'admin', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Milan', 'Pavlović', 2, '/images/profiles/default.png', 'Platform administrator.', '+381601000001', NULL, NULL, NULL, 'Belgrade', 'Nemanjina 11', NULL, true, NOW() - INTERVAL '180 days'),

('sofia.markovic@bookwise.local', 'sofia.markovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Sofia', 'Marković', 1, '/images/profiles/default.png', 'Beauty specialist.', '+381601000101', 'Sofia Beauty Studio', 'Beauty', 'Hair, makeup and skincare.', 'Belgrade', 'Knez Mihailova 18', 'https://sofiabeauty.example', true, NOW() - INTERVAL '160 days'),
('nikola.ivanovic@bookwise.local', 'nikola.ivanovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Nikola', 'Ivanović', 1, '/images/profiles/default.png', 'Barber and stylist.', '+381601000102', 'Urban Barber House', 'Barber', 'Classic and modern cuts.', 'Novi Sad', 'Bulevar Oslobođenja 63', 'https://urbanbarber.example', true, NOW() - INTERVAL '145 days'),
('mina.jovic@bookwise.local', 'mina.jovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Mina', 'Jović', 1, '/images/profiles/default.png', 'Nails and lashes expert.', '+381601000103', 'Mina Nails & Lashes', 'Nails', 'Gel, BIAB and lash sets.', 'Niš', 'Obrenovićeva 27', 'https://minanails.example', true, NOW() - INTERVAL '132 days'),
('igor.petrovic@bookwise.local', 'igor.petrovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Igor', 'Petrović', 1, '/images/profiles/default.png', 'Fitness coach.', '+381601000104', 'Forge Fitness Lab', 'Fitness', 'Strength and conditioning.', 'Kragujevac', 'Kralja Petra 22', 'https://forgefitness.example', true, NOW() - INTERVAL '120 days'),
('tamara.savic@bookwise.local', 'tamara.savic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Tamara', 'Savić', 1, '/images/profiles/default.png', 'Yoga and mindfulness.', '+381601000105', 'Zen Flow Studio', 'Wellness', 'Yoga, mobility and meditation.', 'Novi Sad', 'Dunavska 12', 'https://zenflow.example', true, NOW() - INTERVAL '116 days'),
('lazar.stankovic@bookwise.local', 'lazar.stankovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Lazar', 'Stanković', 1, '/images/profiles/default.png', 'Device repair specialist.', '+381601000106', 'TechFix Point', 'Repair', 'Phone and laptop repair.', 'Belgrade', 'Bulevar kralja Aleksandra 102', 'https://techfixpoint.example', true, NOW() - INTERVAL '140 days'),
('jovana.vukovic@bookwise.local', 'jovana.vukovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Jovana', 'Vuković', 1, '/images/profiles/default.png', 'Home cleaning services.', '+381601000107', 'CleanNest', 'Cleaning', 'Residential and office cleaning.', 'Subotica', 'Korzo 8', 'https://cleannest.example', true, NOW() - INTERVAL '98 days'),
('petar.kostic@bookwise.local', 'petar.kostic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Petar', 'Kostić', 1, '/images/profiles/default.png', 'Photographer.', '+381601000108', 'Frame Studio', 'Photography', 'Portraits and events.', 'Belgrade', 'Cara Lazara 59', 'https://framestudio.example', true, NOW() - INTERVAL '110 days'),
('andjela.djordjevic@bookwise.local', 'andjela.djordjevic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Anđela', 'Đorđević', 1, '/images/profiles/default.png', 'Math and programming tutoring.', '+381601000109', 'Smart Tutor Hub', 'Tutoring', 'STEM tutoring for high school and college.', 'Niš', 'Vizantijski bulevar 35', 'https://smarttutor.example', true, NOW() - INTERVAL '90 days'),
('uros.rakic@bookwise.local', 'uros.rakic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Uroš', 'Rakić', 1, '/images/profiles/default.png', 'Plumbing and maintenance.', '+381601000110', 'PlumbLine', 'Plumbing', 'Installations and emergency repairs.', 'Čačak', 'Gradski trg 2', 'https://plumbline.example', true, NOW() - INTERVAL '84 days'),
('milica.tomic@bookwise.local', 'milica.tomic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Milica', 'Tomić', 1, '/images/profiles/default.png', 'Massage therapist.', '+381601000111', 'BodyBalance Studio', 'Massage', 'Relax, deep tissue and sports massage.', 'Belgrade', 'Vračar 44', 'https://bodybalance.example', true, NOW() - INTERVAL '76 days'),
('marko.jankovic@bookwise.local', 'marko.jankovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Marko', 'Janković', 1, '/images/profiles/default.png', 'Electrical services.', '+381601000112', 'VoltCraft', 'Electrical', 'Electrical repairs and installations.', 'Kragujevac', 'Lepenički bulevar 10', 'https://voltcraft.example', true, NOW() - INTERVAL '70 days'),

('ana.ilic@bookwise.local', 'ana.ilic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Ana', 'Ilić', 0, '/images/profiles/default.png', 'Enjoys wellness sessions.', '+381611000001', NULL, NULL, NULL, 'Belgrade', 'Dorćol 3', NULL, true, NOW() - INTERVAL '100 days'),
('stefan.matic@bookwise.local', 'stefan.matic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Stefan', 'Matić', 0, '/images/profiles/default.png', 'Gym and fitness fan.', '+381611000002', NULL, NULL, NULL, 'Novi Sad', 'Liman 2', NULL, true, NOW() - INTERVAL '96 days'),
('jelena.petrovic@bookwise.local', 'jelena.petrovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Jelena', 'Petrović', 0, '/images/profiles/default.png', 'Books makeup and nails often.', '+381611000003', NULL, NULL, NULL, 'Niš', 'Medijana 14', NULL, true, NOW() - INTERVAL '93 days'),
('nemanja.milenkovic@bookwise.local', 'nemanja.milenkovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Nemanja', 'Milenković', 0, '/images/profiles/default.png', 'Tech enthusiast.', '+381611000004', NULL, NULL, NULL, 'Kragujevac', 'Erdoglija 7', NULL, true, NOW() - INTERVAL '88 days'),
('katarina.vasic@bookwise.local', 'katarina.vasic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Katarina', 'Vasić', 0, '/images/profiles/default.png', 'Loves photo sessions.', '+381611000005', NULL, NULL, NULL, 'Belgrade', 'Novi Beograd 45', NULL, true, NOW() - INTERVAL '82 days'),
('luka.savic@bookwise.local', 'luka.savic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Luka', 'Savić', 0, '/images/profiles/default.png', 'Books cleaning and repairs.', '+381611000006', NULL, NULL, NULL, 'Subotica', 'Centar 4', NULL, true, NOW() - INTERVAL '79 days'),
('marija.nikolic@bookwise.local', 'marija.nikolic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Marija', 'Nikolić', 0, '/images/profiles/default.png', 'Skincare and massage regular.', '+381611000007', NULL, NULL, NULL, 'Belgrade', 'Vračar 18', NULL, true, NOW() - INTERVAL '74 days'),
('filip.radic@bookwise.local', 'filip.radic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Filip', 'Radić', 0, '/images/profiles/default.png', 'Car service and tutoring.', '+381611000008', NULL, NULL, NULL, 'Čačak', 'Avenija 2', NULL, true, NOW() - INTERVAL '70 days'),
('ivana.bozic@bookwise.local', 'ivana.bozic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Ivana', 'Božić', 0, '/images/profiles/default.png', 'Yoga and meditation.', '+381611000009', NULL, NULL, NULL, 'Novi Sad', 'Podbara 6', NULL, true, NOW() - INTERVAL '66 days'),
('andrija.stojanovic@bookwise.local', 'andrija.stojanovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Andrija', 'Stojanović', 0, '/images/profiles/default.png', 'Books once per week.', '+381611000010', NULL, NULL, NULL, 'Niš', 'Durlan 8', NULL, true, NOW() - INTERVAL '63 days'),
('teodora.kostic@bookwise.local', 'teodora.kostic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Teodora', 'Kostić', 0, '/images/profiles/default.png', 'Busy schedule, likes quick booking.', '+381611000011', NULL, NULL, NULL, 'Belgrade', 'Karaburma 16', NULL, true, NOW() - INTERVAL '60 days'),
('vladimir.pavic@bookwise.local', 'vladimir.pavic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Vladimir', 'Pavić', 0, '/images/profiles/default.png', 'Repair and electrical services.', '+381611000012', NULL, NULL, NULL, 'Kragujevac', 'Centar 11', NULL, true, NOW() - INTERVAL '58 days'),
('nina.jovanovic@bookwise.local', 'nina.jovanovic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Nina', 'Jovanović', 0, '/images/profiles/default.png', 'Frequent app user.', '+381611000013', NULL, NULL, NULL, 'Belgrade', 'Mirijevo 9', NULL, true, NOW() - INTERVAL '55 days'),
('dusan.ristic@bookwise.local', 'dusan.ristic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Dušan', 'Ristić', 0, '/images/profiles/default.png', 'Needs flexible evening slots.', '+381611000014', NULL, NULL, NULL, 'Novi Sad', 'Adice 10', NULL, true, NOW() - INTERVAL '50 days'),
('sofija.lazic@bookwise.local', 'sofija.lazic', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Sofija', 'Lazić', 0, '/images/profiles/default.png', 'Books wellness and beauty.', '+381611000015', NULL, NULL, NULL, 'Niš', 'Pantelej 5', NULL, true, NOW() - INTERVAL '46 days');
WITH providers AS (
    SELECT "Id"
    FROM "Users"
    WHERE "Role" = 1
)
INSERT INTO "ProviderWorkingHours" ("ProviderId", "DayOfWeek", "IsWorking", "StartTime", "EndTime")
SELECT p."Id", d.day_of_week,
       CASE WHEN d.day_of_week = 0 THEN false ELSE true END,
       CASE
         WHEN d.day_of_week BETWEEN 1 AND 5 THEN INTERVAL '09:00'
         WHEN d.day_of_week = 6 THEN INTERVAL '10:00'
         ELSE NULL
       END,
       CASE
         WHEN d.day_of_week BETWEEN 1 AND 5 THEN INTERVAL '17:00'
         WHEN d.day_of_week = 6 THEN INTERVAL '14:00'
         ELSE NULL
       END
FROM providers p
CROSS JOIN (VALUES (0), (1), (2), (3), (4), (5), (6)) AS d(day_of_week);
INSERT INTO "ProviderServices" (
    "ProviderId", "Name", "Category", "Description", "ImageUrl", "PriceEur", "DurationMinutes", "IsActive", "CreatedAt"
)
SELECT u."Id", s.name, s.category, s.description, '/images/services/default-service.svg', s.price_eur, s.duration_minutes, true, NOW() - (random() * INTERVAL '120 days')
FROM "Users" u
JOIN (
    VALUES
    ('sofia.markovic', 'Luxury Haircut', 'Haircut', 'Wash, cut and style.', 32.00, 60),
    ('sofia.markovic', 'Party Makeup', 'Makeup', 'Evening makeup look.', 45.00, 75),

    ('nikola.ivanovic', 'Classic Fade', 'Barber', 'Fade and beard trim.', 24.00, 45),
    ('nikola.ivanovic', 'Premium Beard Styling', 'Barber', 'Shape and line-up.', 18.00, 30),

    ('mina.jovic', 'Gel Nails Full Set', 'Nails', 'Gel extension and design.', 38.00, 90),
    ('mina.jovic', 'Lash Lift', 'Eyelashes', 'Natural lift and tint.', 27.00, 50),

    ('igor.petrovic', 'Personal Training', 'Personal Training', 'One-on-one strength plan.', 35.00, 60),
    ('igor.petrovic', 'Functional Conditioning', 'Fitness', 'Mobility + cardio.', 28.00, 50),

    ('tamara.savic', 'Private Yoga', 'Fitness', 'Personalized yoga class.', 26.00, 60),
    ('tamara.savic', 'Mindfulness Session', 'Massage', 'Breathwork and relaxation.', 22.00, 45),

    ('lazar.stankovic', 'Laptop Diagnostics', 'Repair', 'Hardware and software checks.', 26.00, 45),
    ('lazar.stankovic', 'Phone Screen Replacement', 'Repair', 'Fast OLED replacement.', 89.00, 60),

    ('jovana.vukovic', 'Apartment Deep Clean', 'Cleaning', 'Kitchen, bathroom, floors.', 58.00, 120),
    ('jovana.vukovic', 'Office Refresh Clean', 'Cleaning', 'Small office cleaning.', 48.00, 100),

    ('petar.kostic', 'Portrait Photo Session', 'Photography', 'Studio portraits.', 65.00, 90),
    ('petar.kostic', 'Event Coverage', 'Photography', 'Up to 2h event shoot.', 150.00, 120),

    ('andjela.djordjevic', 'Math Tutoring', 'Tutoring', 'High school math coaching.', 22.00, 60),
    ('andjela.djordjevic', '.NET Mentoring', 'Tutoring', 'Backend architecture sessions.', 30.00, 60),

    ('uros.rakic', 'Leak Repair', 'Plumbing', 'Emergency leak fix.', 40.00, 60),
    ('uros.rakic', 'Bathroom Installation', 'Plumbing', 'New faucet/sink install.', 75.00, 90),

    ('milica.tomic', 'Relax Massage', 'Massage', 'Full body relaxation.', 36.00, 60),
    ('milica.tomic', 'Sports Massage', 'Massage', 'Recovery-focused massage.', 42.00, 60),

    ('marko.jankovic', 'Electrical Inspection', 'Electrical', 'Apartment safety check.', 34.00, 60),
    ('marko.jankovic', 'Outlet Installation', 'Electrical', 'New socket installation.', 29.00, 45)
) AS s(username, name, category, description, price_eur, duration_minutes)
ON u."Username" = s.username;
WITH clients AS (
    SELECT "Id"
    FROM "Users"
    WHERE "Role" = 0
),
services AS (
    SELECT ps."Id", ps."ProviderId", ps."DurationMinutes"
    FROM "ProviderServices" ps
    WHERE ps."IsActive" = true
),
gen AS (
    SELECT generate_series(1, 260) AS g
),
picked AS (
    SELECT
        g,
        (SELECT s."Id" FROM services s ORDER BY random() LIMIT 1) AS service_id,
        (SELECT c."Id" FROM clients c ORDER BY random() LIMIT 1) AS client_id
    FROM gen
),
valid_rows AS (
    SELECT p.g, p.service_id, p.client_id, s."DurationMinutes", s."ProviderId"
    FROM picked p
    JOIN services s ON s."Id" = p.service_id
    WHERE p.client_id <> s."ProviderId"
),
times AS (
    SELECT
        vr.service_id,
        vr.client_id,
        vr."DurationMinutes",
        (
            date_trunc('hour', NOW() - INTERVAL '35 days' + (vr.g * INTERVAL '4 hours'))
            + ((vr.g % 4) * INTERVAL '15 minutes')
        ) AS start_utc
    FROM valid_rows vr
)
INSERT INTO "Appointments" ("ProviderServiceId", "ClientId", "StartUtc", "EndUtc", "Status", "CreatedAt")
SELECT
    t.service_id,
    t.client_id,
    t.start_utc,
    t.start_utc + (t."DurationMinutes" * INTERVAL '1 minute'),
    CASE
        WHEN t.start_utc > NOW() THEN 0
        WHEN random() < 0.72 THEN 2
        ELSE 1
    END,
    t.start_utc - INTERVAL '2 days'
FROM times t
LIMIT 210;
WITH clients AS (
    SELECT "Id" FROM "Users" WHERE "Role" = 0
),
providers AS (
    SELECT "Id" FROM "Users" WHERE "Role" = 1
),
pairs AS (
    SELECT
        LEAST(c."Id", p."Id") AS user1_id,
        GREATEST(c."Id", p."Id") AS user2_id,
        NOW() - (random() * INTERVAL '30 days') AS created_at
    FROM clients c
    CROSS JOIN providers p
    ORDER BY random()
    LIMIT 90
)
INSERT INTO "Chats" ("User1Id", "User2Id", "CreatedAtUtc", "UpdatedAtUtc")
SELECT user1_id, user2_id, created_at, created_at + INTERVAL '1 day'
FROM pairs
ON CONFLICT ("User1Id", "User2Id") DO NOTHING;

WITH selected_chats AS (
    SELECT "Id", "User1Id", "User2Id"
    FROM "Chats"
    ORDER BY "CreatedAtUtc" DESC
    LIMIT 90
),
msg_gen AS (
    SELECT generate_series(1, 4) AS i
)
INSERT INTO "ChatMessages" ("ChatId", "SenderId", "IsRead", "Content", "SentAtUtc")
SELECT
    ch."Id",
    CASE WHEN mg.i % 2 = 0 THEN ch."User1Id" ELSE ch."User2Id" END,
    (mg.i <= 2),
    CASE mg.i
        WHEN 1 THEN 'Zdravo, da li imate slobodan termin ove nedelje?'
        WHEN 2 THEN 'Imam, odgovara mi četvrtak ili petak popodne.'
        WHEN 3 THEN 'Super, hajde četvrtak u 17h.'
        ELSE 'Dogovoreno, poslata je potvrda termina.'
    END,
    NOW() - INTERVAL '8 days' + (mg.i * INTERVAL '6 hours') + (random() * INTERVAL '6 days')
FROM selected_chats ch
CROSS JOIN msg_gen mg
LIMIT 240;
INSERT INTO "Notifications" ("UserId", "Type", "Title", "Message", "IsRead", "RelatedUserId", "ChatMessageId", "CreatedAtUtc")
SELECT
    CASE WHEN cm."SenderId" = c."User1Id" THEN c."User2Id" ELSE c."User1Id" END,
    'chat',
    'Nova poruka',
    LEFT(cm."Content", 120),
    false,
    cm."SenderId",
    cm."Id",
    cm."SentAtUtc"
FROM "ChatMessages" cm
JOIN "Chats" c ON c."Id" = cm."ChatId"
ORDER BY cm."SentAtUtc" DESC
LIMIT 180;

INSERT INTO "Notifications" ("UserId", "Type", "Title", "Message", "IsRead", "RelatedUserId", "ChatMessageId", "CreatedAtUtc")
SELECT
    ps."ProviderId",
    'appointment',
    'Novi termin',
    CONCAT('Zakazan je ', ps."Name", ' za ', to_char(a."StartUtc", 'DD Mon YYYY HH24:MI')),
    false,
    a."ClientId",
    NULL,
    a."CreatedAt"
FROM "Appointments" a
JOIN "ProviderServices" ps ON ps."Id" = a."ProviderServiceId"
ORDER BY a."CreatedAt" DESC
LIMIT 160;

COMMIT;
