## Section 14: Data Tables & Asset Naming Conventions

### 14.1 Merge Chains Table

| Chain_ID | Level | Item_Name_EN | Item_Name_AR | Visual_Description | Rarity | Sell_Price | Spawn_Weight |
|----------|-------|--------------|--------------|-------------------|--------|------------|--------------| 
| coffee | 1 | Coffee Bean | حبة بن | "Single brown coffee bean, shiny and round" | Common | 1 | 50% |
| coffee | 2 | Bean Pile | كومة حبوب | "Small pile of 3 roasted coffee beans" | Common | 2 | 30% |
| coffee | 3 | Ground Coffee | بن مطحون | "Wooden bowl filled with fine brown powder" | Uncommon | 5 | 15% |
| coffee | 4 | Coffee Cup | فنجان قهوة | "Steaming decorated ceramic coffee cup" | Uncommon | 10 | 5% |
| coffee | 5 | Coffee Pot | دلّة قهوة | "Polished brass Arabic coffee pot with engraving" | Rare | 25 | 0% |
| tea | 1 | Tea Leaf | ورقة شاي | "Single green tea leaf, fresh" | Common | 1 | 50% |
| tea | 2 | Tea Bundle | حزمة شاي | "Small bundle of dried tea leaves tied with twine" | Common | 2 | 30% |
| tea | 3 | Tea Bag | كيس شاي | "Cute fabric tea bag with a tag" | Uncommon | 5 | 15% |
| tea | 4 | Tea Glass | كأس شاي | "Golden-rimmed glass cup of amber tea with steam" | Uncommon | 10 | 5% |
| tea | 5 | Tea Set | طقم شاي | "Complete ornate tea set on small brass tray" | Rare | 25 | 0% |
| travel | 1 | Torn Paper | ورقة ممزقة | "Small torn piece of yellowed paper" | Common | 1 | 40% |
| travel | 2 | Paper Scraps | قصاصات ورق | "Several paper scraps pieced together" | Common | 2 | 25% |
| travel | 3 | Old Letter | رسالة قديمة | "Folded letter with faded handwriting and wax seal" | Uncommon | 5 | 15% |
| travel | 4 | Envelope Bundle | حزمة رسائل | "Bundle of envelopes tied with ribbon" | Uncommon | 10 | 5% |
| travel | 5 | Treasure Map | خريطة كنز | "Hand-drawn world map with marked routes" | Rare | 25 | 0% |
| travel | 6 | Explorer's Journal | دفتر المستكشف | "Leather-bound journal with maps, sketches, pressed flowers" | Epic | 50 | 0% |
| tools | 1 | Fabric Scrap | قطعة قماش | "Small beige fabric scrap" | Common | 1 | 40% |
| tools | 2 | Rope | حبل | "Coiled brown rope" | Common | 2 | 25% |
| tools | 3 | Magnifying Glass | عدسة مكبرة | "Brass-framed magnifying glass" | Uncommon | 5 | 15% |
| tools | 4 | Excavation Brush | فرشاة حفريات | "Small artifact-cleaning brush" | Uncommon | 10 | 5% |
| tools | 5 | Chisel | أزميل | "Small chisel with wooden handle" | Rare | 25 | 0% |
| tools | 6 | Explorer's Bag | حقيبة مستكشف | "Leather bag containing all tools" | Epic | 50 | 0% |

### 14.2 Level Rewards Table

| Level | Coins_80 | Coins_90 | Coins_100 | Gems_100 | Item_Reward | XP | Unlocks_Feature | Narrative_ID |
|-------|----------|----------|-----------|----------|-------------|-----|-----------------|-------------|
| 1 | 50 | 75 | 100 | 5 | First Coffee Seed | 100 | - | nar_001 |
| 2 | 75 | 110 | 150 | 7 | Old Map (Part 1/3) | 150 | Shop | nar_002 |
| 3 | 100 | 150 | 200 | 10 | Golden Key | 200 | Passport | nar_003 |
| 4 | 125 | 180 | 250 | 12 | - | 250 | - | nar_004 |
| 5 | 175 | 245 | 350 | 14 | First Tool | 350 | Tools Chain | nar_005 |
| 6 | 200 | 280 | 400 | 15 | - | 400 | - | nar_006 |
| 7 | 225 | 315 | 450 | 16 | - | 450 | - | nar_007 |
| 8 | 250 | 350 | 500 | 17 | - | 500 | - | nar_008 |
| 9 | 275 | 385 | 550 | 18 | - | 550 | - | nar_009 |
| 10 | 300 | 420 | 600 | 50 | Exclusive Skin | 600 | Mini-Games | nar_010 |
| 15 | 425 | 595 | 850 | 23 | Lantern Item | 850 | Dark Tiles | nar_015 |
| 20 | 550 | 770 | 1100 | 100 | Strategist Title | 1100 | Move Limits | nar_020 |
| 25 | 675 | 945 | 1350 | 100 | Rare Card | 1350 | - | nar_025 |
| 30 | 800 | 1120 | 1600 | 100 | Bomb ×5 | 1600 | Chain Reactions | nar_030 |
| 40 | 1050 | 1470 | 2100 | 100 | Legendary Skin | 2100 | Element Evo | nar_040 |
| 50 | 1300 | 1820 | 2600 | 500 | Master Explorer Title + Legendary Skin | 2600 | Arc 2 Tease | nar_050 |

### 14.3 Obstacles Table

| Obstacle_ID | Name_EN | Name_AR | Description | Unlock_Condition | Visual_State_1 | Visual_State_2 | Sound_Effect |
|-------------|---------|---------|-------------|------------------|----------------|----------------|-------------|
| obs_lock | Locked Tile | مربع مقفل | Wooden crate with iron lock | Merge Lv3+ adjacent | "Wooden crate with iron lock, chains" | "Lock breaking animation, chains falling" | sfx_wood_break |
| obs_ice | Frozen Tile | مربع مجمد | Transparent blue ice layer | Merge ×2 adjacent | "Thick blue ice layer with frost patterns" | "Ice cracking, shattering" | sfx_ice_crack |
| obs_keylock | Key Lock Tile | مربع مفتاح | Special keyhole marking | Requires Tool (Key) item | "Ornate keyhole with golden trim" | "Key turning, door opening glow" | sfx_key_turn |
| obs_dark | Dark Tile | مربع مظلم | Completely dark tile | Merge light item adjacent | "Solid black with shadow wisps" | "Light spreading outward" | sfx_light_burst |
| obs_gold | Golden Tile | مربع ذهبي | Glowing golden tile (bonus) | Normal reveal | "Pulsing golden glow" | "Golden burst with coins" | sfx_gold_chime |

### 14.4 Powerups Table

| Powerup_ID | Name_EN | Name_AR | Effect_Description | Coin_Price | Gem_Price | Max_Hold | Icon_Ref |
|------------|---------|---------|--------------------|------------|-----------|----------|----------|
| pu_fog | Fog Clearer | كاشف الضباب | Reveals 3–5 random tiles instantly | 200 | 10 | 10 | icon_fog |
| pu_hammer | Golden Hammer | المطرقة الذهبية | Opens any locked or frozen tile | 300 | 15 | 5 | icon_hammer |
| pu_bomb | Bomb | القنبلة | Reveals all adjacent tiles in radius | 400 | 20 | 3 | icon_bomb |
| pu_hint | Hint | تلميح | Highlights a mergeable pair | 100 | 5 | 10 | icon_bulb |
| pu_time | Time Extender | وقت إضافي | +10 moves in Move Limit levels | 150 | 8 | 5 | icon_clock |
| pu_vision | Vision | الرؤية | 3-second peek at complete image | 100 | 5 | 5 | icon_eye |

### 14.5 Hidden Images Table

| Image_ID | Level | Name_EN | Name_AR | Theme | Artist_Prompt | Grid_Size | Difficulty | Reward_ID |
|----------|-------|---------|---------|-------|---------------|-----------|------------|-----------|
| img_001 | 1 | Tea Corner | ركن القهوة | Nomad Cafe | "Cozy café corner, wooden table, brass teapot, glass cups, lavender vase, warm sunlight, cozy cartoon style, 1024x1024" | 3×3 | Easy | reward_lv1 |
| img_002 | 2 | Old Shelf | الرف القديم | Nomad Cafe | "Dusty wooden shelf, old books, postcards, brass compass, folded map, dried flowers, sepia photo, cozy cartoon style, 1024x1024" | 3×3 | Easy | reward_lv2 |
| img_003 | 3 | Street Window | نافذة الشارع | Nomad Cafe | "Ornate café window, old town street view, potted cactus, coffee cup, notebook, bicycle, sunset, cozy cartoon style, 1024x1024" | 4×3 | Medium-Easy | reward_lv3 |

### 14.6 Achievements Table

| Ach_ID | Name_EN | Name_AR | Description | Condition | Reward_Type | Reward_Value | Icon |
|--------|---------|---------|-------------|-----------|-------------|--------------|------|
| ach_begin | The Beginning | البداية | Complete Level 1 | Complete Lv1 | Coins | 100 | icon_flag |
| ach_saver | The Saver | المقتصد | Collect 1,000 Coins | Coins >= 1000 | Gems | 10 | icon_piggy |
| ach_merge10 | Merge Master | سيد الدمج | Merge 100 items total | Total merges >= 100 | Coins | 200 | icon_merge |
| ach_reveal | The Revealer | الكاشف | Reveal 10 complete images | 100% images >= 10 | Gems | 50 | icon_picture |
| ach_speed | Speed Runner | العداء السريع | Complete any level under 2 min | Any level < 120s | Gems | 20 | icon_timer |
| ach_perfect | Perfectionist | المثالي | Get 3 stars on 10 levels | 3-star levels >= 10 | Gems | 100 | icon_star3 |
| ach_collector | Collector | الجامع | Fill 1 album page (10 images) | Album page complete | Coins + Gems | 500 + 50 | icon_album |
| ach_explorer | Master Explorer | المستكشف الأعظم | Complete all 50 levels at 100% | All levels 100% | Title + Skin | Legendary | icon_crown |

### 14.7 Daily Challenges Pool

| Challenge_ID | Type | Description_EN | Description_AR | Target | Reward_Coins | Reward_Gems | Difficulty |
|--------------|------|---------------|----------------|--------|--------------|-------------|-----------|
| daily_merge | Merge | "Merge {target} items" | "ادمج {target} عنصر" | 50 | 50 | 0 | Easy |
| daily_level | Level | "Complete {target} levels" | "أكمل {target} مستويات" | 3 | 100 | 2 | Medium |
| daily_stars | Stars | "Earn {target} stars" | "اجمع {target} نجوم" | 5 | 75 | 3 | Medium |
| daily_reveal | Reveal | "Reveal {target} tiles" | "اكشف {target} بلاطة" | 30 | 60 | 1 | Easy |
| daily_chain | Chain | "Create a Level {target} item" | "اصنع عنصر مستوى {target}" | 5 | 150 | 5 | Hard |
| daily_perfect | Perfect | "Get 3 stars on {target} level(s)" | "احصل على 3 نجوم في {target} مستوى" | 1 | 100 | 5 | Hard |
| daily_nohelp | No Help | "Complete a level without powerups" | "أكمل مستوى بدون مساعدات" | 1 | 120 | 5 | Hard |

---

### 14.8 Asset Naming Conventions

All game assets must follow these naming conventions for consistency and pipeline efficiency.

**General Format:** `category_subcategory_name_variant_size`

| Asset Type | Prefix | Example | Notes |
|-----------|--------|---------|-------|
| Hidden Images | `img_` | `img_001_tea_corner_1024.png` | Sequential ID + short name |
| Merge Items | `item_` | `item_coffee_lv1_bean.png` | Chain name + level + item |
| UI Icons | `icon_` | `icon_hammer.png` | Short descriptive name |
| Backgrounds | `bg_` | `bg_grid_wood.png` | Category + variant |
| Sound Effects | `sfx_` | `sfx_merge_success.wav` | Action + state |
| Music | `bgm_` | `bgm_level_calm_01.ogg` | Context + variant number |
| VFX / Particles | `vfx_` | `vfx_reveal_gold_sparkle.prefab` | Effect + descriptor |
| Character Icons | `char_` | `char_laith_curious.png` | Name + emotion |
| Skins | `skin_` | `skin_coffee_golden.png` | Base item + variant |
| Obstacle Sprites | `obs_` | `obs_lock_locked.png` | Type + state |
| Tutorial Assets | `tut_` | `tut_hand_pointer.png` | Element name |

**File Format Standards:**

| Asset Type | Format | Max Size | Resolution |
|-----------|--------|----------|-----------|
| Hidden Images | PNG (transparent) or JPG (solid bg) | 500 KB | 1024×1024 |
| UI Sprites | PNG with transparency | 100 KB | As needed |
| Icons | PNG with transparency | 50 KB | 128×128 or 256×256 |
| Sound Effects | WAV (source) / OGG (build) | 200 KB | 44.1 kHz |
| Music | OGG | 3 MB | 44.1 kHz |

---

## Asset List (Levels 1–3)

### Images (12 assets)

| Asset Name | Type | Description |
|-----------|------|-------------|
| img_001_tea_corner_1024.png | Hidden Image | Level 1 — Grandma's tea corner |
| img_002_old_shelf_1024.png | Hidden Image | Level 2 — The old dusty shelf |
| img_003_street_window_1024.png | Hidden Image | Level 3 — Café window to old street |
| item_coffee_lv1_bean.png | Merge Item | Coffee bean (Level 1) |
| item_coffee_lv2_pile.png | Merge Item | Bean pile (Level 2) |
| item_coffee_lv3_ground.png | Merge Item | Ground coffee (Level 3) |
| item_coffee_lv4_cup.png | Merge Item | Coffee cup (Level 4) |
| item_coffee_lv5_pot.png | Merge Item | Coffee pot (Level 5) |
| item_tea_lv1_leaf.png | Merge Item | Tea leaf (Level 1) |
| item_tea_lv2_bundle.png | Merge Item | Tea bundle (Level 2) |
| item_tea_lv3_bag.png | Merge Item | Tea bag (Level 3) |
| item_tea_lv4_glass.png | Merge Item | Tea glass (Level 4) |

### Additional Items for Level 3 (6 assets)

| Asset Name | Type |
|-----------|------|
| item_tea_lv5_set.png | Tea set |
| item_travel_lv1_paper.png | Torn paper |
| item_travel_lv2_scraps.png | Paper scraps |
| item_travel_lv3_letter.png | Old letter |
| item_travel_lv4_bundle.png | Envelope bundle |
| item_travel_lv5_map.png | Treasure map |

### UI & Effects (15 assets)

| Asset Name | Type |
|-----------|------|
| icon_coin.png | Gold coin icon |
| icon_gem.png | Gem icon |
| icon_star.png | Star icon |
| icon_fog.png | Fog Clearer icon |
| icon_hammer.png | Golden Hammer icon |
| icon_bulb.png | Hint icon |
| icon_bomb.png | Bomb icon |
| vfx_merge_sparkle.prefab | Merge effect |
| vfx_reveal_gold.prefab | Reveal gold particles |
| vfx_confetti.prefab | Celebration confetti |
| char_laith_curious.png | Laith — curious expression |
| char_laith_excited.png | Laith — excited expression |
| char_grandma_warm.png | Grandma — warm expression |
| char_grandma_mysterious.png | Grandma — mysterious expression |
| tut_hand_pointer.png | Tutorial hand pointer |

### Audio (10 assets)

| Asset Name | Description |
|-----------|-------------|
| sfx_merge_success.ogg | Successful merge sound |
| sfx_reveal_tile.ogg | Tile reveal chime |
| sfx_wood_break.ogg | Locked tile breaking |
| sfx_ice_crack.ogg | Frozen tile cracking |
| sfx_gold_chime.ogg | Golden tile reward |
| sfx_celebration.ogg | 100% completion fanfare |
| sfx_button_tap.ogg | UI button tap |
| sfx_dialogue_blip.ogg | Dialogue text blip |
| bgm_level_calm_01.ogg | In-level calm music |
| bgm_menu_cozy_01.ogg | Menu/map screen music |

---

## Action Plan — What To Do First

| Step | Action | Priority |
|------|--------|----------|
| 1 | Finalize art style with concept artist — create 1 hidden image + 5 merge items as style reference | 🔴 Critical |
| 2 | Build Level 1 prototype in Unity — Grid + Merge + Reveal working | 🔴 Critical |
| 3 | Commission/generate all Level 1–3 hidden images using AI art prompts | 🟡 High |
| 4 | Design and produce all merge item sprites for Levels 1–3 | 🟡 High |
| 5 | Implement SaveSystem and EconomyManager | 🟡 High |
| 6 | Record/source placeholder sound effects | 🟢 Medium |
| 7 | Build UI screens (main menu, level select, shop) | 🟢 Medium |
| 8 | Implement dialogue system with Level 1–3 scripts | 🟢 Medium |
| 9 | Internal playtest with 5–10 testers | 🟢 Medium |
| 10 | Prepare store listing materials (screenshots, description) | 🔵 Later |

---

*End of Game Design Document — Sip & Seek: Nomad Cafe v1.0*
