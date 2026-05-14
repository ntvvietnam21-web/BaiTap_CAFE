CREATE DATABASE QL_CuaHangNuoc PRIMARY(
	Name = QLCH_PRIMARY,
	Filename = 'E:\LuuDuLieu\QLCH_PRIMARY.mdf',
	Size = 15MB,
	Maxsize = 30MB,
	Filegrowth = 10%)
LOG ON(
	Name = QLCH_LOG,
	Filename = 'E:\LuuDuLieu\QLCH_LOG.ldf',		
	Size = 15MB,
	Maxsize = 30MB,
	Filegrowth = 10%);
GO

USE QL_CuaHangNuoc;
GO


CREATE TABLE LOAI_MON (
	MaLoai CHAR(10) NOT NULL PRIMARY KEY,
	TenLoai NVARCHAR(50)
);

CREATE TABLE MENU (
	MaMon CHAR(10) NOT NULL PRIMARY KEY,
	TenMon NVARCHAR(50),
	DonGia INT,
	MaLoai CHAR(10),
	FOREIGN KEY (MaLoai) REFERENCES LOAI_MON(MaLoai)
);

CREATE TABLE KHACHHANG (
	MaKH CHAR(10) NOT NULL PRIMARY KEY,
	HoTen NVARCHAR(50),
	NgaySinh DATE,
	DienThoai VARCHAR(15),
	LoaiThanhVien NVARCHAR(20) 
);

CREATE TABLE HOADON (
	MaHD INT IDENTITY(1,1) PRIMARY KEY,
	MaKH CHAR(10),
	MaMon CHAR(10),
	NgayBan DATETIME DEFAULT GETDATE(),
	SoLuong INT,
	FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
	FOREIGN KEY (MaMon) REFERENCES MENU(MaMon)
);
GO

INSERT INTO LOAI_MON VALUES 
('L01', N'Cà phê pha máy'),
('L02', N'Cà phê truyền thống'),
('L03', N'Trà trái cây'),
('L04', N'Trà sữa'),
('L05', N'Đá xay (Ice Blended)'),
('L06', N'Nước ép tươi'),
('L07', N'Sinh tố'),
('L08', N'Trà nóng'),
('L09', N'Soda mix'),
('L10', N'Bánh ngọt ăn kèm');

INSERT INTO MENU VALUES 
('M01', N'Espresso', 25000, 'L01'),
('M02', N'Cappuccino', 45000, 'L01'),
('M03', N'Cà phê đen đá', 20000, 'L02'),
('M04', N'Cà phê sữa đá', 25000, 'L02'),
('M05', N'Trà đào cam sả', 35000, 'L03'),
('M06', N'Trà dâu tằm', 38000, 'L03'),
('M07', N'Trà sữa trân châu', 30000, 'L04'),
('M08', N'Hồng trà sữa', 30000, 'L04'),
('M09', N'Matcha đá xay', 45000, 'L05'),
('M10', N'Nước ép dưa hấu', 30000, 'L06'),
('M11', N'Nước ép cam', 35000, 'L06'),
('M12', N'Sinh tố bơ', 40000, 'L07'),
('M13', N'Trà hoa cúc nóng', 30000, 'L08'),
('M14', N'Soda chanh dây', 35000, 'L09'),
('M15', N'Bánh Tiramisu', 40000, 'L10');

INSERT INTO KHACHHANG VALUES 
('KH01', N'Nguyễn Văn An', '1995-10-10', '0901234567', N'VIP'),
('KH02', N'Lê Thị Bình', '2000-05-20', '0907654321', N'Thân thiết'),
('KH03', N'Trần Thanh Tùng', '1998-12-05', '0912345678', N'Thường'),
('KH04', N'Phạm Hoài Phong', '1992-03-15', '0923456789', N'VIP'),
('KH05', N'Ngô Đình Việt', '2004-09-27', '0934567890', N'Thân thiết'),
('KH06', N'Trần Thị Liễu', '2003-02-18', '0945678901', N'Thường'),
('KH07', N'Đỗ Thị Hạnh', '1997-11-28', '0956789012', N'VIP'),
('KH08', N'Lê Văn Khánh', '1999-01-18', '0967890123', N'Thân thiết'),
('KH09', N'Nguyễn Thị Lan', '2001-07-15', '0978901234', N'Thường'),
('KH10', N'Trương Thị Huệ', '2002-08-31', '0989012345', N'Thân thiết'),
('KH11', N'Hoàng Văn Nam', '1996-04-22', '0990123456', N'Thường'),
('KH12', N'Vũ Tố Anh', '2005-06-12', '0801234567', N'VIP');

INSERT INTO HOADON (MaKH, MaMon, NgayBan, SoLuong) VALUES 
('KH01', 'M04', '2023-10-25 08:30:00', 2),
('KH01', 'M15', '2023-10-25 08:30:00', 1),
('KH02', 'M05', '2023-10-25 09:15:00', 1),
('KH03', 'M07', '2023-10-25 10:00:00', 3),
('KH04', 'M01', '2023-10-25 10:30:00', 1),
('KH05', 'M09', '2023-10-26 14:20:00', 2),
('KH06', 'M11', '2023-10-26 15:00:00', 1),
('KH07', 'M02', '2023-10-26 16:45:00', 2),
('KH08', 'M12', '2023-10-27 19:00:00', 1),
('KH09', 'M10', '2023-10-27 19:30:00', 2),
('KH10', 'M06', '2023-10-28 08:00:00', 1),
('KH11', 'M08', '2023-10-28 09:15:00', 4),
('KH12', 'M14', '2023-10-28 13:20:00', 1),
('KH01', 'M03', '2023-10-29 07:30:00', 1),
('KH04', 'M13', '2023-10-29 20:10:00', 2);
GO

CREATE TABLE NGUYENLIEU (
    MaNL CHAR(10) PRIMARY KEY,
    TenNL NVARCHAR(50),
    DonViTinh NVARCHAR(20), 
    TonKho DECIMAL(10,2) DEFAULT 0,
    MucCanhBao DECIMAL(10,2) DEFAULT 0 
);

CREATE TABLE CONGTHUC (
    MaMon CHAR(10),
    MaNL CHAR(10),
    SoLuongTieuHao DECIMAL(10,2), 
    PRIMARY KEY (MaMon, MaNL),
    FOREIGN KEY (MaMon) REFERENCES MENU(MaMon),
    FOREIGN KEY (MaNL) REFERENCES NGUYENLIEU(MaNL)
);

INSERT INTO NGUYENLIEU (MaNL, TenNL, DonViTinh, TonKho, MucCanhBao) VALUES 
('NL01', N'Cà phê hạt xay', 'gram', 5000, 1000), 
('NL02', N'Sữa đặc', 'ml', 3000, 500),
('NL03', N'Trà đen', 'gram', 800, 200),
('NL04', N'Trân châu', 'gram', 1500, 500);

INSERT INTO CONGTHUC (MaMon, MaNL, SoLuongTieuHao) VALUES 
('M01', 'NL01', 18), 
('M04', 'NL01', 20), ('M04', 'NL02', 30),
('M07', 'NL03', 10), ('M07', 'NL02', 40), ('M07', 'NL04', 50);

CREATE TABLE NguoiDung (
    TenDangNhap VARCHAR(50) PRIMARY KEY,
    MatKhau VARCHAR(50),
    VaiTro NVARCHAR(20)
);
INSERT INTO NguoiDung 
VALUES 
('admin', 'abc', 'Admin'),
('NguyenTruongVy', '123', 'Admin'), 
('TranNguyenAnhTuan', '456', 'Admin'), 
('nv01', 'nv1', N'Nhân viên'),
('nv02', 'nv2', N'Nhân viên');
