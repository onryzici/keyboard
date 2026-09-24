# Little Switch

Sıcak, stilize bir klavye atölyesinde geçen oynanabilir Unity prototipi.

## Açılış

Git LFS kurulu olmalı (`git lfs install`). Klonladıktan sonra `git lfs pull` çalıştırın. Unity Hub'da **LittleSwitch/** klasörünü Unity **6000.5.6f1** ile açın. HDRP paketleri yüklendikten sonra `Assets/Scenes/WorkshopFocused.unity` sahnesinde Play'e basın.

**WASD** yürü, **fare** bak. Tezgâha yaklaşınca **E** ile çalış; **Esc** ile ayağa kalk. Açık terminal veya eldeki parça varsa Esc önce onu kapatır/bırakır. Yürürken Esc imleci bırakır, sol tık yürüyüşe döner. Windows sürümü 4K tam ekran açılır. Güncel değişiklikler: [WORKSHOP_FOCUS.md](WORKSHOP_FOCUS.md).

Önceki `Workshop.unity` ve yalnızca ortam denemesi `WalkableWorkshop.unity` korunur. Sipariş ve fiziksel montaj güncel sahnede de çalışır; aşağıdaki etkileşimler tezgâh modunda kullanılabilir.

## Kontroller

- **Bilgisayara tıkla:** siparişi oku, parçaları seç ve satın al.
- **Raftaki ilgili pakete tıkla:** masaya getir, aç. İçinden switch veya tuşu sürükleyip klavyeye yerleştir.
- **Alt + sol tuşla sürükle:** masa eşyalarını ve açılmış parça kutularını taşı. Yerleşim kaydedilir; masa dışına, klavyeye veya diğer taşınabilir eşyalara çakışan bırakmalar geri alınır. Mat ve montaj klavyesi sabittir.
- **Sağ tuşla sürükle:** atölye ve montaj görünümünde sınırlı kamera dönüşü.
- **Fare tekerleği:** yumuşak yaklaşma / uzaklaşma.
- **Tab:** atölye / montaj görünümü.
- **Tuşa kısa sağ tık:** parçayı çıkar veya testte sorunlu pini düzelt. Masadaki sökücü ve pense de kullanılabilir.
- **Bardağa tıkla:** sıcak içeceğinden bir yudum al.
- **Esc:** taşımayı iptal et, eldeki aleti bırak veya görünümü değiştir.

Tamamlanan klavyeyi test et, paketle ve teslim et. Beş müşteri siparişi, ödeme, itibar ve raf yükseltmesi içeren ilk oynanabilir döngü mevcuttur. Kaydedilmiş switch sesleri ve CC0 lo-fi müzik kullanılır.

İlerleme `Application.persistentDataPath/little-switch-v1.json`, masa yerleşimi `desk-layout-v1.json` içinde saklanır. Kişisel kayıtlar repoya dahil değildir.

## Geliştirme ve doğrulama

Kod: `LittleSwitch/Assets/LittleSwitch/Scripts/`. Kurallar/veri kontrolü: Unity menüsünde **Little Switch > Validate Core Loop**. Son etkileşim kontrolleri `references/checkpoints/verify-cozy-interactions.cs.txt`, `test-desk-layout.cs.txt` ve `test-desk-input.cs.txt` içindedir; Unity CLI `eval_file` üzerinden Play modunda çalıştırılır, oyuncu kayıtları geri yüklenir.

Sahne güncel düzenin kaynağıdır. Eski `Create Workshop` / toplu kompozisyon üretim araçları güncel sahnenin üzerine çalıştırılmamalıdır. Sanat yönü: `AGENTS.md`; sahne notları: `references/SCENE_NOTES.md`.

Üçüncü taraf model, doku, ses ve müzik lisansları: [SOURCES.md](LittleSwitch/Assets/LittleSwitch/ThirdParty/SOURCES.md). Bu repo üçüncü taraf varlıkların lisanslarını değiştirmez.



