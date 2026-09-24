# Little Switch — HDRP geçişi ve ışıklandırma

Proje: `keyboard/LittleSwitch`  
Sahne: `Assets/Scenes/Workshop.unity`  
Unity: 6000.5.6f1 · HDRP: 17.5.0

## Kullanılan ayarlar

| Özellik | Ayar |
| --- | --- |
| Ana güneş | 1 Directional Light, 16.000 lux, 5250 K, Euler (26, 81, 0) |
| Güneş boyutu | 1,2° açısal çap |
| Güneş volumetrikleri | Etkin, volumetric dimmer 2,2; volumetric shadow dimmer 1 |
| Güneş gölgeleri | HDRP High / PCSS, 2048 çözünürlük, 4 cascade, 45 m mesafe |
| Contact shadows | Yalnızca güneş; uzunluk 0,12 m, opacity 0,65, mesafe 25 m, fade 5 m |
| Gökyüzü | Gradient Sky, Dynamic ambient; Exposure modu 10,5; serin mavi/gri üst ve orta renkler |
| Global Volume | `Little Switch HDRP atmosphere`, global, priority 10 |
| Global Fog | Gerçek volumetrik sis açık; mean free path 100 m, yükseklik 0–14 m |
| Yerel sis | `HDRP window scattering`; mean free path 35 m; boyut (15, 4,8, 6,4) m; kenarlarda yumuşak geçiş |
| Anisotropy | 0,58, global fog üzerinde |
| Sis kalitesi | Manual, ekran çözünürlüğünün %25'i, 128 derinlik dilimi, depth extent 40 m |
| Sis dolaylı ışığı | Light probe dimmer 0,3; multiple scattering 0,15 |
| SSAO | Intensity 0,5; radius 0,28 m; direct lighting strength 0,1 |
| Dolaylı ışık | HDRP SSGI, Ray Marching, tam çözünürlük, 64 adım, denoising; environment fallback |
| Exposure | Fixed EV100 10, compensation 0; otomatik pozlama yok |
| Tonemapping | ACES |
| Bloom | Intensity 0,06, threshold 2, scatter 0,6 |
| Pratik ışık | Mevcut raf feneri: Point, 3600 K, 900 lumen temel güç, 4 m range, mevcut hafif titreşim |
| Fener volumetrikleri | Dimmer 0,03 |
| Aktif ışık sayısı | **2: 1 Directional + 1 Point**; önceki 6 yardımcı ışık kapalı |
| Kamera | Mevcut oyun kamerası, HDRP camera data, TAA; volumetrics/SSGI/SSAO/contact shadows açık |

SSGI ekranda görülen yüzeylerden dolaylı ışık üretir. Bu geçişte baked lightmap üretilmedi; ekran dışındaki ayrıntılı ışık sekmeleri bu yöntemin sınırıdır.

## Geçiş kapsamı

- Graphics Settings ve her iki Quality seviyesi `WorkshopPipeline.asset` kullanır.
- 330 malzeme HDRP'ye uyarlandı. Renk ve doku referansları korundu; kaynak modeller değiştirilmedi.
- `SoftShapes.Mat` çalışma sırasında üretilen klavye ve parça malzemelerini HDRP/Lit ile oluşturur.
- Mevcut fenerin emission property adı, kupa buharı ve toz shader'ları HDRP için uyarlandı.
- Eski `SoftWindowRay` mesh efektlerinin renderer'ları kapatıldı. Işık huzmeleri HDRP tarafından hesaplanır.
- Pencere dışındaki mevcut gökyüzü panelinin gölge dökmesi kapatıldı; güneşi engelleyen sorun buydu.
- Oda düzeni, mesh referansları, model konumları ve ölçekleri kontrol edildi. Oynanış/etkileşim davranışı değiştirilmedi.
- Windows açılış kontrolünde mevcut `ShopCatalog.asset` dosyasının script referansının boş olduğu yakalandı. `ShopCatalog` sınıfı kendi dosyasına taşındı ve asset referansı bağlandı; veri alanları ve oyun kuralları değişmedi.
- Eski sahne üretim araçlarının derlenebilmesi için URP paketi arşiv bağımlılığı olarak durur; aktif oyun renderer'ı HDRP'dir.

## Doğrulama

Play Mode'da gerçek `ShopGame.viewCamera` üzerinden masa, güneşe çapraz bakış, ışık yolu, karanlık raf bölgesi ve yakın montaj görünümü incelendi. İnceleme sırasında kamera konumları geçici değiştirildi; sahneye kaydedilmedi.

- Çalışan pipeline: `UnityEngine.Rendering.HighDefinition.HDRenderPipeline`.
- Görünen malzemeler destekleniyor; URP/hata shader'ı bulunmadı.
- Klavye seçimi: 61/61 tuş.
- Oyun kamerasının Volume Stack'inde volumetrik sis etkin.
- Görüntüler: `LittleSwitch/Logs/hdrp-1.png` ... `hdrp-5.png` (sRGB).
- Kontrol çıktısı: `LittleSwitch/Logs/hdrp-validation.txt`.
- Windows HDRP derlemesi: `Builds/WindowsHDRP/Little Switch.exe`.
- Son Windows derlemesi 0 hatayla tamamlandı; çalışan EXE'de katalog, arayüz, mevcut kayıt ve klavye yüklendi. Açılış logunda exception veya eksik script yok.
- Çalışan Windows oyunu: `LittleSwitch/Logs/hdrp-player-final.png`; açılış logu: `LittleSwitch/Logs/hdrp-player-final.log`.

## Düzenleme

Işık/atmosfer profili: `Assets/Settings/LittleSwitchHDRP/WorkshopAtmosphere.asset`.

Editor menüsü `Little Switch > HDRP > Apply workshop lighting` mevcut sahneye bu belgede yazılı ışık ayarlarını uygular. `HDRPWorkshopReview` yalnızca editörde doğrulama, görüntü alma ve Windows derlemesi için kullanılır; oyuncuya ek sistem yüklemez.
