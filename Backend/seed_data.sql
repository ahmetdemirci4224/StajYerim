-- StajYerim Projesi - BIGG Demo Veritabanı (Final Fix)

-- 1. Eski Verileri Temizle
TRUNCATE TABLE "Users" CASCADE;
TRUNCATE TABLE "Skills" CASCADE;

-- 2. Yetenekleri (Skills) Ekle
INSERT INTO "Skills" ("Name", "CreatedDate", "IsActive", "IsDeleted") VALUES 
('C#', CURRENT_TIMESTAMP, true, false), ('.NET Core', CURRENT_TIMESTAMP, true, false), ('SQL', CURRENT_TIMESTAMP, true, false), ('Java', CURRENT_TIMESTAMP, true, false), ('Spring Boot', CURRENT_TIMESTAMP, true, false), ('PostgreSQL', CURRENT_TIMESTAMP, true, false), ('Python', CURRENT_TIMESTAMP, true, false), ('Django', CURRENT_TIMESTAMP, true, false), ('Machine Learning', CURRENT_TIMESTAMP, true, false), ('React', CURRENT_TIMESTAMP, true, false), ('TypeScript', CURRENT_TIMESTAMP, true, false), ('Tailwind CSS', CURRENT_TIMESTAMP, true, false), ('C++', CURRENT_TIMESTAMP, true, false), ('Node.js', CURRENT_TIMESTAMP, true, false), ('Express', CURRENT_TIMESTAMP, true, false), ('MongoDB', CURRENT_TIMESTAMP, true, false), ('Go', CURRENT_TIMESTAMP, true, false), ('Kubernetes', CURRENT_TIMESTAMP, true, false), ('Docker', CURRENT_TIMESTAMP, true, false), ('Angular', CURRENT_TIMESTAMP, true, false), ('Unity', CURRENT_TIMESTAMP, true, false), ('3D Modelleme', CURRENT_TIMESTAMP, true, false), ('PHP', CURRENT_TIMESTAMP, true, false), ('MySQL', CURRENT_TIMESTAMP, true, false), ('Swift', CURRENT_TIMESTAMP, true, false), ('iOS Development', CURRENT_TIMESTAMP, true, false), ('Kotlin', CURRENT_TIMESTAMP, true, false), ('Android Studio', CURRENT_TIMESTAMP, true, false), ('Firebase', CURRENT_TIMESTAMP, true, false), ('Vue.js', CURRENT_TIMESTAMP, true, false), ('GraphQL', CURRENT_TIMESTAMP, true, false), ('Pandas', CURRENT_TIMESTAMP, true, false), ('TensorFlow', CURRENT_TIMESTAMP, true, false), ('Ruby', CURRENT_TIMESTAMP, true, false), ('HTML', CURRENT_TIMESTAMP, true, false), ('CSS', CURRENT_TIMESTAMP, true, false), ('JavaScript', CURRENT_TIMESTAMP, true, false), ('Rust', CURRENT_TIMESTAMP, true, false), ('C', CURRENT_TIMESTAMP, true, false), ('Linux', CURRENT_TIMESTAMP, true, false), ('FastAPI', CURRENT_TIMESTAMP, true, false), ('AWS', CURRENT_TIMESTAMP, true, false), ('REST API', CURRENT_TIMESTAMP, true, false), ('Network Security', CURRENT_TIMESTAMP, true, false), ('Git', CURRENT_TIMESTAMP, true, false), ('IoT', CURRENT_TIMESTAMP, true, false), ('RTOS', CURRENT_TIMESTAMP, true, false), ('PL/pgSQL', CURRENT_TIMESTAMP, true, false), ('NLP', CURRENT_TIMESTAMP, true, false), ('PyTorch', CURRENT_TIMESTAMP, true, false), ('Apache Airflow', CURRENT_TIMESTAMP, true, false);

-- 3. Şirket Kullanıcılarını Ekle (Role: 1 = Company)
INSERT INTO "Users" ("Email", "PasswordHash", "FullName", "Role", "IsEmailVerified", "CreatedDate", "IsActive", "IsDeleted") VALUES
('info@techselcuklu.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'TechSelçuklu A.Ş.', 1, true, CURRENT_TIMESTAMP, true, false),
('ik@innovatech.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'InnovaTech Solutions', 1, true, CURRENT_TIMESTAMP, true, false),
('hello@cloudsys.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'CloudSys Bilişim', 1, true, CURRENT_TIMESTAMP, true, false),
('data@datacore.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'DataCore Analitik', 1, true, CURRENT_TIMESTAMP, true, false),
('security@securenet.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'SecureNet Siber Güvenlik', 1, true, CURRENT_TIMESTAMP, true, false),
('apps@globalmobil.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Global Mobil Apps', 1, true, CURRENT_TIMESTAMP, true, false),
('hr@eticaret.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'E-Ticaret Dinamikleri', 1, true, CURRENT_TIMESTAMP, true, false),
('jobs@oyuntek.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'OyunTek Studio', 1, true, CURRENT_TIMESTAMP, true, false),
('iot@smartcity.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'SmartCity IoT', 1, true, CURRENT_TIMESTAMP, true, false),
('arge@gelecektech.com', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'GelecekTech Ar-Ge', 1, true, CURRENT_TIMESTAMP, true, false);

-- 4. Şirket Profillerini Ekle
INSERT INTO "CompanyProfiles" ("UserId", "CompanyName", "Industry") VALUES
((SELECT "Id" FROM "Users" WHERE "Email"='info@techselcuklu.com'), 'TechSelçuklu A.Ş.', 'Yazılım ve Bilişim'),
((SELECT "Id" FROM "Users" WHERE "Email"='ik@innovatech.com'), 'InnovaTech Solutions', 'Finansal Teknolojiler (Fintech)'),
((SELECT "Id" FROM "Users" WHERE "Email"='hello@cloudsys.com'), 'CloudSys Bilişim', 'Bulut Bilişim ve SaaS'),
((SELECT "Id" FROM "Users" WHERE "Email"='data@datacore.com'), 'DataCore Analitik', 'Yapay Zeka ve Veri Bilimi'),
((SELECT "Id" FROM "Users" WHERE "Email"='security@securenet.com'), 'SecureNet Siber Güvenlik', 'Siber Güvenlik'),
((SELECT "Id" FROM "Users" WHERE "Email"='apps@globalmobil.com'), 'Global Mobil Apps', 'Mobil Yazılım'),
((SELECT "Id" FROM "Users" WHERE "Email"='hr@eticaret.com'), 'E-Ticaret Dinamikleri', 'E-Ticaret Altyapıları'),
((SELECT "Id" FROM "Users" WHERE "Email"='jobs@oyuntek.com'), 'OyunTek Studio', 'Oyun Geliştirme (Gaming)'),
((SELECT "Id" FROM "Users" WHERE "Email"='iot@smartcity.com'), 'SmartCity IoT', 'Nesnelerin İnterneti (IoT)'),
((SELECT "Id" FROM "Users" WHERE "Email"='arge@gelecektech.com'), 'GelecekTech Ar-Ge', 'Ar-Ge ve Gömülü Sistemler');

-- 5. Öğrenci Kullanıcılarını Ekle (Role: 0 = Student)
INSERT INTO "Users" ("Email", "PasswordHash", "FullName", "Role", "IsEmailVerified", "CreatedDate", "IsActive", "IsDeleted") VALUES
('ahmet@selcuk.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Ahmet Yılmaz', 0, true, CURRENT_TIMESTAMP, true, false),
('ayse@odtu.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Ayşe Demir', 0, true, CURRENT_TIMESTAMP, true, false),
('mehmet@itu.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Mehmet Kaya', 0, true, CURRENT_TIMESTAMP, true, false),
('fatma@ege.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Fatma Çelik', 0, true, CURRENT_TIMESTAMP, true, false),
('caner@hacettepe.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Caner Şahin', 0, true, CURRENT_TIMESTAMP, true, false),
('elif@bilkent.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Elif Yıldız', 0, true, CURRENT_TIMESTAMP, true, false),
('burak@boun.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Burak Arslan', 0, true, CURRENT_TIMESTAMP, true, false),
('zeynep@deu.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Zeynep Doğan', 0, true, CURRENT_TIMESTAMP, true, false),
('emre@ktun.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Emre Koç', 0, true, CURRENT_TIMESTAMP, true, false),
('ceren@ankara.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Ceren Öztürk', 0, true, CURRENT_TIMESTAMP, true, false),
('oguzhan@yildiz.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Oğuzhan Aydın', 0, true, CURRENT_TIMESTAMP, true, false),
('merve@gazi.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Merve Kılıç', 0, true, CURRENT_TIMESTAMP, true, false),
('ali@marmara.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Ali Polat', 0, true, CURRENT_TIMESTAMP, true, false),
('busra@sabanci.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Büşra Şen', 0, true, CURRENT_TIMESTAMP, true, false),
('gokhan@gtu.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Gökhan Kaplan', 0, true, CURRENT_TIMESTAMP, true, false),
('gizem@uludag.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Gizem Turan', 0, true, CURRENT_TIMESTAMP, true, false),
('hakan@ktu.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Hakan Erdem', 0, true, CURRENT_TIMESTAMP, true, false),
('seda@anadolu.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Seda Bulut', 0, true, CURRENT_TIMESTAMP, true, false),
('tolga@iyte.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Tolga Çetin', 0, true, CURRENT_TIMESTAMP, true, false),
('deniz@gsu.edu.tr', '$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu', 'Deniz Aktaş', 0, true, CURRENT_TIMESTAMP, true, false);

-- 6. Öğrenci Profillerini Ekle
INSERT INTO "StudentProfiles" ("UserId", "University", "Department", "Gpa") VALUES
((SELECT "Id" FROM "Users" WHERE "Email"='ahmet@selcuk.edu.tr'), 'Selçuk Üniversitesi', 'Bilgisayar Mühendisliği', 3.2),
((SELECT "Id" FROM "Users" WHERE "Email"='ayse@odtu.edu.tr'), 'Orta Doğu Teknik Üniversitesi', 'Yazılım Mühendisliği', 3.8),
((SELECT "Id" FROM "Users" WHERE "Email"='mehmet@itu.edu.tr'), 'İstanbul Teknik Üniversitesi', 'Bilgisayar Mühendisliği', 3.5),
((SELECT "Id" FROM "Users" WHERE "Email"='fatma@ege.edu.tr'), 'Ege Üniversitesi', 'Bilgisayar Mühendisliği', 3.1),
((SELECT "Id" FROM "Users" WHERE "Email"='caner@hacettepe.edu.tr'), 'Hacettepe Üniversitesi', 'Yazılım Mühendisliği', 3.4),
((SELECT "Id" FROM "Users" WHERE "Email"='elif@bilkent.edu.tr'), 'Bilkent Üniversitesi', 'Bilgisayar Bilimleri', 3.9),
((SELECT "Id" FROM "Users" WHERE "Email"='burak@boun.edu.tr'), 'Boğaziçi Üniversitesi', 'Bilgisayar Mühendisliği', 3.7),
((SELECT "Id" FROM "Users" WHERE "Email"='zeynep@deu.edu.tr'), 'Dokuz Eylül Üniversitesi', 'Yazılım Mühendisliği', 3.0),
((SELECT "Id" FROM "Users" WHERE "Email"='emre@ktun.edu.tr'), 'Konya Teknik Üniversitesi', 'Bilgisayar Mühendisliği', 3.3),
((SELECT "Id" FROM "Users" WHERE "Email"='ceren@ankara.edu.tr'), 'Ankara Üniversitesi', 'Bilgisayar Mühendisliği', 3.6),
((SELECT "Id" FROM "Users" WHERE "Email"='oguzhan@yildiz.edu.tr'), 'Yıldız Teknik Üniversitesi', 'Yazılım Mühendisliği', 3.2),
((SELECT "Id" FROM "Users" WHERE "Email"='merve@gazi.edu.tr'), 'Gazi Üniversitesi', 'Bilgisayar Mühendisliği', 3.5),
((SELECT "Id" FROM "Users" WHERE "Email"='ali@marmara.edu.tr'), 'Marmara Üniversitesi', 'Yazılım Mühendisliği', 3.1),
((SELECT "Id" FROM "Users" WHERE "Email"='busra@sabanci.edu.tr'), 'Sabancı Üniversitesi', 'Bilgisayar Mühendisliği', 3.8),
((SELECT "Id" FROM "Users" WHERE "Email"='gokhan@gtu.edu.tr'), 'Gebze Teknik Üniversitesi', 'Bilgisayar Mühendisliği', 3.4),
((SELECT "Id" FROM "Users" WHERE "Email"='gizem@uludag.edu.tr'), 'Bursa Uludağ Üniversitesi', 'Yazılım Mühendisliği', 3.0),
((SELECT "Id" FROM "Users" WHERE "Email"='hakan@ktu.edu.tr'), 'Karadeniz Teknik Üniversitesi', 'Bilgisayar Mühendisliği', 3.3),
((SELECT "Id" FROM "Users" WHERE "Email"='seda@anadolu.edu.tr'), 'Anadolu Üniversitesi', 'Bilgisayar Mühendisliği', 3.6),
((SELECT "Id" FROM "Users" WHERE "Email"='tolga@iyte.edu.tr'), 'İzmir Yüksek Teknoloji Enstitüsü', 'Yazılım Mühendisliği', 3.7),
((SELECT "Id" FROM "Users" WHERE "Email"='deniz@gsu.edu.tr'), 'Galatasaray Üniversitesi', 'Bilgisayar Bilimleri', 3.9);

-- 7. Öğrenci Yeteneklerini Ekle (1=Temel, 2=Orta, 3=Ileri)
INSERT INTO "StudentSkills" ("StudentId", "SkillId", "ProficiencyLevel", "CreatedDate") VALUES
((SELECT "Id" FROM "Users" WHERE "Email"='ahmet@selcuk.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='C#'), 3, CURRENT_TIMESTAMP),
((SELECT "Id" FROM "Users" WHERE "Email"='ahmet@selcuk.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='SQL'), 2, CURRENT_TIMESTAMP),
((SELECT "Id" FROM "Users" WHERE "Email"='ayse@odtu.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='Java'), 3, CURRENT_TIMESTAMP),
((SELECT "Id" FROM "Users" WHERE "Email"='ayse@odtu.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='PostgreSQL'), 2, CURRENT_TIMESTAMP),
((SELECT "Id" FROM "Users" WHERE "Email"='mehmet@itu.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='Python'), 3, CURRENT_TIMESTAMP),
((SELECT "Id" FROM "Users" WHERE "Email"='mehmet@itu.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='Django'), 2, CURRENT_TIMESTAMP),
((SELECT "Id" FROM "Users" WHERE "Email"='fatma@ege.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='React'), 3, CURRENT_TIMESTAMP),
((SELECT "Id" FROM "Users" WHERE "Email"='fatma@ege.edu.tr'), (SELECT "Id" FROM "Skills" WHERE "Name"='Tailwind CSS'), 2, CURRENT_TIMESTAMP);

-- 8. Staj İlanlarını Ekle
INSERT INTO "InternshipPostings" ("CompanyId", "Title", "Description", "Location", "WorkModel", "Status", "CreatedDate", "ExpiresAt", "IsActive", "IsDeleted") VALUES
((SELECT "Id" FROM "Users" WHERE "Email"='info@techselcuklu.com'), 'Backend Geliştirici Stajyeri', 'Kurumsal projelerimizde .NET altyapısında görev alacak ve RESTful API geliştirmelerine destek olacak stajyer arıyoruz.', 'Konya', 'Hybrid', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='info@techselcuklu.com'), 'Frontend Geliştirici Stajyeri', 'Modern web uygulamalarımızın arayüzlerini React ile geliştirecek, takım çalışmasına yatkın yenilikçi stajyerler arıyoruz.', 'Konya', 'Remote', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='ik@innovatech.com'), 'Fintech Yazılım Stajyeri', 'Ödeme sistemleri projelerimizde mikroservis mimarisini öğrenmek ve Java ile yüksek ölçekli kod yazmak isteyen takım arkadaşı.', 'İstanbul', 'On-Site', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='hello@cloudsys.com'), 'Bulut Sistemleri Stajyeri', 'AWS ve Azure üzerinde sistem konfigürasyonu ve CI/CD otomasyonu konusunda deneyim kazanacak DevOps stajyeri.', 'Ankara', 'Remote', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='data@datacore.com'), 'Veri Bilimi Stajyeri', 'Büyük veri setleri üzerinde makine öğrenmesi modelleri eğitecek ve Python kullanarak analitik çözümler üretecek stajyer.', 'İzmir', 'Hybrid', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='security@securenet.com'), 'Siber Güvenlik Analist Stajyeri', 'Sızma testleri (Penetration Testing) ve ağ güvenliği konusunda kendini geliştirmek isteyen, araştırmacı stajyerler aranıyor.', 'Ankara', 'On-Site', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='apps@globalmobil.com'), 'iOS Geliştirici Stajyeri', 'Milyonlarca indirilen mobil e-ticaret uygulamamızın iOS tarafında Swift ile geliştirmeler yapacak mobil yazılım stajyeri.', 'İstanbul', 'Hybrid', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='apps@globalmobil.com'), 'Android Geliştirici Stajyeri', 'Uygulamamızın Android sürümünde Kotlin mimarisi ile modern arayüzler ve servis entegrasyonları yapacak yetenekli stajyer.', 'İstanbul', 'Remote', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='hr@eticaret.com'), 'Fullstack Web Stajyeri', 'E-ticaret platformumuzun hem backend (Node.js) hem de frontend (Vue.js) taraflarında full-stack görev alacak dinamik stajyer.', 'Bursa', 'Hybrid', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='jobs@oyuntek.com'), 'Oyun Teknoloji Stajyeri', 'Mobil ve PC platformlarına yönelik geliştirdiğimiz oyunlarda C# ve Unity kullanarak mekanik geliştirecek takım arkadaşı.', 'Eskişehir', 'On-Site', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='iot@smartcity.com'), 'IoT Yazılım Stajyeri', 'Akıllı şehir projelerimizde gömülü sistemler ve sensör haberleşme protokolleri üzerine C/C++ ile çalışacak gömülü yazılım stajyeri.', 'Kocaeli', 'On-Site', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='arge@gelecektech.com'), 'Ar-Ge Gömülü Yazılım Stajyeri', 'Savunma sanayii odaklı Ar-Ge projelerimizde mikrodenetleyici programlama yapacak, elektronik ve donanıma ilgi duyan stajyer.', 'Ankara', 'Hybrid', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='ik@innovatech.com'), 'Veritabanı Yönetimi Stajyeri', 'Büyük ölçekli veritabanı performans optimizasyonları ve PL/pgSQL yazımı konularında deneyim kazanacak stajyer.', 'İstanbul', 'Remote', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='data@datacore.com'), 'Yapay Zeka Mühendisi Stajyeri', 'Doğal dil işleme (NLP) modelleri üzerine ve LLM entegrasyonları konusunda Ar-Ge yapacak meraklı stajyer aranıyor.', 'İzmir', 'Hybrid', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false),
((SELECT "Id" FROM "Users" WHERE "Email"='hello@cloudsys.com'), 'Data Engineer Stajyeri', 'Veri ambarı mimarileri ve veri boru hatları (Data Pipeline) oluşturma konularında çalışacak veri mühendisi stajyeri.', 'Ankara', 'Remote', 'Open', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '30 days', true, false);

-- 9. İlan Yeteneklerini Ekle
INSERT INTO "PostingSkills" ("PostingId", "SkillId", "IsMandatory") VALUES
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Backend Geliştirici Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='C#'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Frontend Geliştirici Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='React'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Fintech Yazılım Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='Java'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Bulut Sistemleri Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='AWS'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Veri Bilimi Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='Python'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Siber Güvenlik Analist Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='Network Security'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='iOS Geliştirici Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='Swift'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Android Geliştirici Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='Kotlin'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Fullstack Web Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='Vue.js'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Oyun Teknoloji Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='Unity'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='IoT Yazılım Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='C++'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Ar-Ge Gömülü Yazılım Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='C'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Veritabanı Yönetimi Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='PostgreSQL'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Yapay Zeka Mühendisi Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='NLP'), true),
((SELECT "Id" FROM "InternshipPostings" WHERE "Title"='Data Engineer Stajyeri'), (SELECT "Id" FROM "Skills" WHERE "Name"='SQL'), true);
