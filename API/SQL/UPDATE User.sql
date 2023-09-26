-- UPDATE User
-- SET
--     Address = CASE
--         WHEN Id = 1 THEN 'Quan Binh Tan, TP. Ho Chi Minh'
--         WHEN Id = 2 THEN 'Quan Binh Chanh, TP. Ho Chi Minh'
--         WHEN Id = 3 THEN 'Quan Tan Phu, TP. Ho Chi Minh'
--         WHEN Id = 4 THEN 'Quan Phu Nhuan, TP. Ho Chi Minh'
--         WHEN Id = 5 THEN 'Quan Go Vap, TP. Ho Chi Minh'
--         WHEN Id = 6 THEN 'Quan Thu Duc, TP. Ho Chi Minh'
--         WHEN Id = 7 THEN 'Quan 1, TP. Ho Chi Minh'
--         WHEN Id = 8 THEN 'Quan 2, TP. Ho Chi Minh'
--         WHEN Id = 9 THEN 'Quan 3, TP. Ho Chi Minh'
--         WHEN Id = 10 THEN 'Quan 4, TP. Ho Chi Minh'
--         WHEN Id = 11 THEN 'Quan 5, TP. Ho Chi Minh'
--         WHEN Id = 12 THEN 'Quan 6, TP. Ho Chi Minh'
--         WHEN Id = 13 THEN 'Quan 7, TP. Ho Chi Minh'
--         WHEN Id = 14 THEN 'Quan 8, TP. Ho Chi Minh'
--         WHEN Id = 15 THEN 'Quan 9, TP. Ho Chi Minh'
--         WHEN Id = 16 THEN 'Quan 10, TP. Ho Chi Minh'
--         WHEN Id = 17 THEN 'Quan 11, TP. Ho Chi Minh'
--         WHEN Id = 18 THEN 'Quan 12, TP. Ho Chi Minh'
--         WHEN Id = 19 THEN 'Quan Tan Binh, TP. Ho Chi Minh'
--         WHEN Id = 20 THEN 'Quan Tan Phong, TP. Ho Chi Minh'
--         ELSE Address  -- For rows other than 1 to 20, keep the existing Address
--     END;


INSERT INTO Workers_Chores (WorkerId, ChoreId, Version)
VALUES 
    (1, 1, '00000000-0000-0000-0000-000000000000'),
    (1, 2, '00000000-0000-0000-0000-000000000000'),
    (1, 3, '00000000-0000-0000-0000-000000000000'),
    (2, 2, '00000000-0000-0000-0000-000000000000'),
    (2, 5, '00000000-0000-0000-0000-000000000000'),
    (2, 4, '00000000-0000-0000-0000-000000000000'),
    (3, 1, '00000000-0000-0000-0000-000000000000'),
(3, 2, '00000000-0000-0000-0000-000000000000'),
(4, 1, '00000000-0000-0000-0000-000000000000'),
(4, 2, '00000000-0000-0000-0000-000000000000'),
(4, 3, '00000000-0000-0000-0000-000000000000'),
(5, 1, '00000000-0000-0000-0000-000000000000'),
(5, 2, '00000000-0000-0000-0000-000000000000'),
(6, 1, '00000000-0000-0000-0000-000000000000'),
(6, 2, '00000000-0000-0000-0000-000000000000'),
(7, 1, '00000000-0000-0000-0000-000000000000'),
(7, 2, '00000000-0000-0000-0000-000000000000'),
(7, 3, '00000000-0000-0000-0000-000000000000'),
(7, 4, '00000000-0000-0000-0000-000000000000'),
(8, 1, '00000000-0000-0000-0000-000000000000'),
(8, 2, '00000000-0000-0000-0000-000000000000'),
(8, 3, '00000000-0000-0000-0000-000000000000'),
(8, 4, '00000000-0000-0000-0000-000000000000'),
(8, 5, '00000000-0000-0000-0000-000000000000'),
(9, 1, '00000000-0000-0000-0000-000000000000'),
(9, 2, '00000000-0000-0000-0000-000000000000'),
(9, 3, '00000000-0000-0000-0000-000000000000'),
    (10, 1, '00000000-0000-0000-0000-000000000000'),
    (10, 3, '00000000-0000-0000-0000-000000000000'),
    (10, 4, '00000000-0000-0000-0000-000000000000'),
    (11, 2, '00000000-0000-0000-0000-000000000000'),
    (11, 5, '00000000-0000-0000-0000-000000000000'),
    (11, 4, '00000000-0000-0000-0000-000000000000'),
    (12, 1, '00000000-0000-0000-0000-000000000000'),
    (12, 2, '00000000-0000-0000-0000-000000000000'),
    (13, 1, '00000000-0000-0000-0000-000000000000'),
    (14, 3, '00000000-0000-0000-0000-000000000000'),
    (15, 4, '00000000-0000-0000-0000-000000000000')
;

INSERT INTO OrderHistory (Date, GuestAddress, GuestEmail, GuestName, GuestPhone, WorkerId)
VALUES 
    ('2023-09-10', 'Quan 10, TP. Ho Chi Minh', 'olivia.davis@example.com', 'Olivia Davis', '555-789-4321', 1),
    ('2023-09-09', 'Quan 11, TP. Ho Chi Minh', 'william.smith@example.com', 'William Smith', '555-567-8901', 1),
    ('2023-09-08', 'Quan 12, TP. Ho Chi Minh', 'sophia.taylor@example.com', 'Sophia Taylor', '555-432-1098', 1),
    ('2023-09-07', 'Quan Tan Binh, TP. Ho Chi Minh', 'daniel.brown@example.com', 'Daniel Brown', '555-123-9876', 1),
    ('2023-09-06', 'Quan Tan Phong, TP. Ho Chi Minh', 'emma.wilson@example.com', 'Emma Wilson', '555-987-2345', 1),
    ('2023-09-05', 'Binh Tan Tp. Ho Chi Minh', 'HoangHuy@example.com', 'William Smith', '0896423546', 1);

INSERT INTO Reviews (Id, Content, Date, Rate, Version)
VALUES 
(11,'Lam rat co tam, trach nhiem', '2023-09-10', 5,'00000000-0000-0000-0000-000000000000'),
(12,'Rat thich cach lam viec chuyen nghiep', '2023-09-09', 4,'00000000-0000-0000-0000-000000000000'),
(13,'Tot, dung gio, sach se', '2023-09-08', 5,'00000000-0000-0000-0000-000000000000'),
(14,'Sach se, ngan nap', '2023-09-07', 4,'00000000-0000-0000-0000-000000000000'),
(15,'Rat Ok <3', '2023-09-05', 5,'00000000-0000-0000-0000-000000000000'),
(16,'Nha sang bong khong 1 vet do', '2023-09-10', 4,'00000000-0000-0000-0000-000000000000'),
(17,'Rat tuyet voi luon', '2023-09-07', 5,'00000000-0000-0000-0000-000000000000'),
(18,'Lan sau se book tiep', '2023-09-06', 4,'00000000-0000-0000-0000-000000000000'),
(19,'Don nha rat sach, nhung ton nhieu thoi gian qua, con hoi vung ve trong khau lau nha', '2023-09-05', 3,'00000000-0000-0000-0000-000000000000'),
(20,'Cung tam on', '2023-09-09', 3,'00000000-0000-0000-0000-000000000000'),
(21,'Ngon lanh lien', '2023-09-08', 4,'00000000-0000-0000-0000-000000000000'),
(22,'Lan sau se book tiep', '2023-09-07', 5,'00000000-0000-0000-0000-000000000000'),
(23,'Lan sau se book tiep', '2023-09-06', 5,'00000000-0000-0000-0000-000000000000');
