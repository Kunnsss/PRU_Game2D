# 🎮 PRU Game
---

##  Mục Lục

- [Giới Thiệu](#giới-thiệu)
- [Yêu Cầu Hệ Thống](#yêu-cầu-hệ-thống)
- [Cài Đặt](#cài-đặt)
- [Cấu Trúc Dự Án](#cấu-trúc-dự-án)
- [Công Nghệ Sử Dụng](#công-nghệ-sử-dụng)

---

##  Giới Thiệu

Mô tả chi tiết hơn về game: thể loại, gameplay, mục tiêu của dự án...

---

##  Yêu Cầu Hệ Thống

| Phần mềm | Phiên bản |
|---|---|
| Unity | 2022.x LTS hoặc mới hơn |
| Visual Studio | 2022 (với workload **Game development with Unity**) |
| .NET | Theo yêu cầu của phiên bản Unity |

---

##  Cài Đặt

### 1. Clone repository

```bash
git clone https://github.com/username/ten-du-an.git
cd ten-du-an
```

### 2. Mở dự án bằng Unity Hub

- Mở **Unity Hub**
- Chọn **Open > Add project from disk**
- Trỏ đến thư mục vừa clone

### 3. Chạy dự án

- Nhấn **Play** trong Unity Editor để chạy thử
- Hoặc vào **File > Build Settings** để build ra sản phẩm

---

## 📁 Cấu Trúc Dự Án

```
 ten-du-an/
├── 📂 Assets/
│   ├── 📂 Scripts/        # Code C# game logic
│   ├── 📂 Scenes/         # Các scene Unity
│   ├── 📂 Prefabs/        # Các prefab tái sử dụng
│   ├── 📂 Art/            # Sprite, texture, model
│   └── 📂 Audio/          # Âm thanh, nhạc nền
├── 📂 Packages/           # Unity Package Manager
├── 📂 ProjectSettings/    # Cấu hình dự án Unity
├── .gitignore
├── .vsconfig
└── README.md
```

---

##  Công Nghệ Sử Dụng

- **Unity** — Game engine
- **C#** — Ngôn ngữ lập trình chính

---

##  Hướng Dẫn Đóng Góp

1. Fork repository này
2. Tạo branch mới: `git checkout -b feature/ten-tinh-nang`
3. Commit thay đổi: `git commit -m "feat: thêm tính năng X"`
4. Push lên branch: `git push origin feature/ten-tinh-nang`
5. Tạo **Pull Request**

>  Không commit các thư mục `Library/`, `Temp/`, `Build/` — đã được bỏ qua trong `.gitignore`.

---


