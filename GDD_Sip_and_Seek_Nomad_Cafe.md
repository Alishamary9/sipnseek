# 🎮 Game Design Document (GDD)
# **Sip & Seek: Nomad Cafe**

**Version:** 1.0  
**Date:** 2026-02-16  
**Document Type:** Master Game Design Document  
**Confidentiality:** Internal Use Only  

---

## Table of Contents

1. [Section 1: Game Identity & Core Info](#section-1-game-identity--core-info)
2. [Section 2: Selected Game Theme](#section-2-selected-game-theme)
3. [Section 3: Detailed Level Design (Levels 1–3)](#section-3-detailed-level-design)
4. [Section 4: Obstacles & Special Items](#section-4-obstacles--special-items)
5. [Section 5: Comprehensive Difficulty Curve](#section-5-comprehensive-difficulty-curve)
6. [Section 6: Monetization Strategy](#section-6-monetization-strategy)
7. [Section 7: Internal Economy](#section-7-internal-economy)
8. [Section 8: Narrative & Storytelling](#section-8-narrative--storytelling)
9. [Section 9: Micro-Dialogues](#section-9-micro-dialogues)
10. [Section 10: Expansion & Future Content](#section-10-expansion--future-content)
11. [Section 11: Visual Guidelines](#section-11-visual-guidelines)
12. [Section 12: Testing & Analytics](#section-12-testing--analytics)
13. [Section 13: Technical Plan](#section-13-technical-plan)
14. [Section 14: Data Tables & Asset Naming](#section-14-data-tables--asset-naming)

---

## Section 1: Game Identity & Core Info

### 1.1 Basic Information

| Field | Detail |
|-------|--------|
| **Game Name** | Sip & Seek: Nomad Cafe |
| **Genre** | Merge Puzzle + Hidden Picture Reveal + Collection Game |
| **Sub-Genre** | Cozy / Relaxing Game |
| **Platform** | Android (initial launch), iOS (3–6 months later) |
| **Engine** | Unity or Godot |
| **Target Audience** | Age 25–45, All genders (60% female lean), Casual Players |
| **Interests** | Calm games, puzzles, culture & travel, art |
| **Play Habits** | During commute, before sleep, work breaks |

### 1.2 Session Duration

| Metric | Duration |
|--------|----------|
| Minimum | 5 minutes (1 level) |
| Average | 10–15 minutes (2–3 levels) |
| Maximum | 30 minutes (intensive session) |

### 1.3 Technical Constraints

| Constraint | Specification |
|-----------|---------------|
| Grid Min Size | 5×5 (Level 1) |
| Grid Max Size | 8×8 |
| Min Touch Target | 44×44 points |
| Connectivity | Fully Offline; Internet only for optional DLC sync, cloud save, rewarded ads |
| App Size Target | < 150 MB initial (10–15 levels); additional content as DLC |
| Performance | 60 FPS on modern devices; 30 FPS acceptable on older (4 GB RAM min) |

### 1.4 Core Gameplay Loop

**1. The Grid:**
- Square grid where mergeable items are placed.
- Grid size increases gradually across levels.

**2. Merge Mechanics:**
- **Method:** Drag & Drop
- **Condition:** Items must be adjacent (horizontal/vertical only — no diagonal)
- **Result:** Two matching items → one higher-level item
- **Placement:** New item appears in the drop cell; the other cell becomes empty
- **No Undo:** Every merge is final (encourages strategic thinking)

**3. Element Generator:**
- Fixed generator at the bottom/side of the grid
- Produces only Level 1 items (unlimited supply)
- Player drags items from the generator to empty cells
- Optional: 5–10 second cooldown between items

**4. Picture Reveal System:**
- A hidden image sits beneath the grid, covered by fog/mask
- Each successful merge reveals part of the image
- **Recommended Reveal Method:** Each merge reveals a tile adjacent to the last revealed tile (logical progressive reveal)

**5. Level Objective:**
- Reveal 80% of the image to complete the level
- Reveal 100% for full bonus (3 Stars)
- No move limit in early levels (to keep the cozy atmosphere)
- Move limits introduced at Level 20+

### 1.5 Design Priorities (Ranked)

| Priority | Description |
|----------|-------------|
| 1. Accessibility | New player understands the game within 30 seconds; interactive tutorial, learn-by-playing |
| 2. Achievement | Positive feedback on every merge (sound, VFX, instant reveal); visible progress bar |
| 3. Cozy Aesthetic | Warm soft colors, no visible timer, calm ASMR-style music, no harsh game-over |
| 4. Replayability | Star rating system, side quests, varying rewards on replay |
| 5. Scalability | System repeatable for 100+ levels; new mechanics added gradually without breaking core |

### 1.6 Inspirations

| Game | What We Love ✅ | What We Avoid ❌ |
|------|----------------|-----------------|
| Merge Mansion | Simple merge mechanic, gradual story, item variety | Annoying energy system |
| Lily's Garden | Engaging story, lovable characters | Traditional Match-3 |
| Hidden Through Time | Discovery fun, beautiful artwork, calm atmosphere | Too passive gameplay |
| Two Dots | Simplicity & elegance, natural difficulty curve | Boring repetition in late levels |

**Hard "No" List:**
- ❌ Forced interstitial ads
- ❌ Artificial difficulty spikes to force payment
- ❌ Severely limited energy system
- ❌ Pay-to-Win mechanics

---

## Section 2: Selected Game Theme

### 2.1 Final Decision: Sip & Seek: Nomad Cafe

| Aspect | Detail |
|--------|--------|
| **Main Title** | "Sip & Seek" — action-oriented (drink & discover), musical rhythm, intriguing |
| **Subtitle** | "Nomad Cafe" — sets the place (café) and theme (travel/nomad) |
| **Expansion Potential** | Future titles: *Sip & Seek: Jungle Bar*, *Sip & Seek: Space Station*, etc. |
| **SEO / Discoverability** | "Seek" targets Hidden Object fans; "Cafe" targets Simulation fans |

### 2.2 Theme Impact on Design

**Story & Characters:**
- Protagonist: Grandchild who inherits the traveling "Nomad Cafe"
- Goal: Restore the café and collect artifacts (Seek) through puzzles (Sip & Relax)

**Visual Elements:**
- Merge items: Café tools (cups, coffee beans, pastries) + Travel tools (maps, compass, suitcases)
- Backgrounds: Change with each café "destination" (Paris, Baghdad, Forest, etc.)

**Scalability:**
- Each Chapter = a new country/environment
- Justifies changing items and art without breaking game logic

---

## Section 3: Detailed Level Design (Levels 1–3)

### 3.1 Art Style Decision

**Chosen Style: Cozy Cartoon Style (Option 1)**

**Reasons:**
1. Best match for the cozy/relaxing vibe the game targets
2. Broad audience appeal — friendly, approachable, non-intimidating
3. Production efficient — consistent style scalable to 100+ images
4. Reference games: Monument Valley, Alto's Adventure

**Style Characteristics:**
- Simple, friendly cartoon illustrations
- Warm, soft color palette
- Smooth, non-harsh outlines
- Soft lighting with no harsh shadows

### 3.2 Reveal System Decision

**Chosen Method: Hybrid (Option C)**
- Image divided into tiles, but reveal uses smooth fade-in animation
- Best of both: clear progress tracking (tiles) + visual beauty (fade)
- Reveal animation: 0.3–0.5 second fade-out of mask per tile

---

### LEVEL 1 — "Grandma's Corner" (ركن الجدة)

#### A. Basic Info

| Field | Value |
|-------|-------|
| Level Number | 1 |
| Difficulty | Very Easy |
| Primary Objective | Reveal 80% of the hidden image |
| Full Completion | 100% reveal for 3 Stars + bonus |
| Expected Merges | 15–20 |
| Estimated Time | 3–5 minutes |
| Secondary Objectives | None (pure tutorial) |

#### B. Grid Design

| Field | Value |
|-------|-------|
| Grid Size | 5×5 = 25 cells |
| All cells usable? | Yes — no blocked or special cells |
| Starting Elements | 3 items (all Level 1) pre-placed |
| Rationale | Smallest grid for first-time learning; max clarity |

#### C. Hidden Image

| Element | Detail |
|---------|--------|
| **Image Name** | Tea Corner |
| **Narrative Description** | The cozy corner where Grandma sat every morning to drink her tea and watch the sunrise. |
| **Visual Description** | A warm café corner with a small wooden table, a polished brass teapot gleaming in the center, two decorated glass cups, a small vase with dried lavender, a woolen blanket draped over a vintage wooden chair, soft golden sunlight streaming in from the left, watercolor-style steam rising from the teapot, patterned tiles on the floor, a faded framed photo on the wall behind. Soft, muted tones with warm highlights. |
| **Dominant Colors** | Coffee Brown (#6D4C41), Warm Beige (#D7CCC8), Brass Gold (#FFD54F), Vintage Rose (#FFAB91), Sage Green (#A5D6A7) |
| **Mood** | Cozy, Nostalgic, Warm, Peaceful |
| **Focal Point** | The gleaming brass teapot at center |
| **Image Grid** | 3×3 = 9 tiles |
| **Resolution** | 1024×1024 px, PNG, < 500 KB |

**AI Art Prompt:**
```
"A cozy corner in a traditional café, small wooden table with a polished brass teapot in the center, two ornate glass tea cups, dried lavender in a small vase, woolen blanket on a vintage wooden chair, soft golden sunlight from the left, warm steam rising, patterned floor tiles, faded photo frame on the wall, cozy cartoon illustration style, warm soft color palette, front-facing eye level view, soft ambient golden lighting, nostalgic peaceful mood, colors: coffee brown, warm beige, brass gold, vintage rose, sage green, high quality digital illustration, 1024x1024"
```

#### D. Reveal Mechanics

| Field | Value |
|-------|-------|
| Reveal Method | Hybrid (tile-based with fade-in animation) |
| Logic | Each merge reveals the tile adjacent to the last revealed tile (progressive flow) |
| Starting Point | Center tile revealed first |
| Obstacles | None |
| VFX on Reveal | Golden particles + satisfying chime + gentle haptic feedback |

#### E. Merge Chains

**Chain 1: Coffee Chain (سلسلة القهوة)**

| Level | Item Name (EN) | Item Name (AR) | Visual Description |
|-------|---------------|----------------|-------------------|
| 1 | Coffee Bean | حبة بن | Single brown coffee bean, shiny and round |
| 2 | Bean Pile | كومة حبوب | Small pile of 3 roasted coffee beans |
| 3 | Ground Coffee | بن مطحون | Wooden bowl filled with fine brown powder |
| 4 | Coffee Cup | فنجان قهوة | A steaming decorated ceramic coffee cup |

**Chain 2: Tea Chain (سلسلة الشاي)**

| Level | Item Name (EN) | Item Name (AR) | Visual Description |
|-------|---------------|----------------|-------------------|
| 1 | Tea Leaf | ورقة شاي | A single green tea leaf, fresh |
| 2 | Tea Bundle | حزمة شاي | Small bundle of dried tea leaves tied with twine |
| 3 | Tea Bag | كيس شاي | A cute fabric tea bag with a tag |
| 4 | Tea Glass | كأس شاي | A golden-rimmed glass cup of amber tea with steam |

#### F. Difficulty & Challenges

| Field | Value |
|-------|-------|
| Expected Merges | 15–20 |
| Move Limit | ∞ (unlimited) |
| Challenges | None — pure tutorial level |
| Element Drop Rate | Generator produces only Level 1 items (100%) |

#### G. Rewards

| Achievement | Reward |
|-------------|--------|
| ⭐ 80% Reveal | 50 Gold Coins |
| ⭐⭐ 90% Reveal | +25 Coins (Total: 75) |
| ⭐⭐⭐ 100% Reveal | +50 Coins + 5 Gems (Total: 100 Coins, 5 Gems) |
| Rare Item | "First Coffee Seed" (used in later levels) |
| Narrative Reward | Grandma's Letter: "This corner was my favorite..." |
| Passport Stamp | Stamp #1 added to Traveler's Passport |

#### H. Big Reveal (100%)

- **Visual Transformation:** Static image becomes animated (3–5 sec): steam rises from teapot, lantern light flickers softly
- **Celebration VFX:** Digital fireworks from image edges, golden confetti, sparkles around frame, haptic celebration pattern, short fanfare music
- **Info Card:** Displays landmark name, rewards, fun fact
- **Share Option:** Save completed image or share to social media with download link
- **Next Level Unlock:** Level 2 auto-unlocks with fog-covered preview

---

### LEVEL 2 — "The Old Shelf" (الرف القديم)

#### A. Basic Info

| Field | Value |
|-------|-------|
| Level Number | 2 |
| Difficulty | Easy |
| Primary Objective | Reveal 80% of the hidden image |
| Full Completion | 100% for 3 Stars |
| Expected Merges | 25–35 |
| Estimated Time | 5–7 minutes |
| Secondary Objective | Complete in under 30 merges |

#### B. Grid Design

| Field | Value |
|-------|-------|
| Grid Size | 5×6 = 30 cells |
| Blocked Cells | None |
| Locked Tiles | 1–2 (require Level 3+ merge adjacent to unlock) |
| Starting Elements | 5 items (4× Level 1, 1× Level 2) |
| Rationale | Slight expansion introduces locked tiles gently |

#### C. Hidden Image

| Element | Detail |
|---------|--------|
| **Image Name** | Old Shelf |
| **Narrative Description** | A dusty old shelf in the back of the café, untouched for years, holding forgotten treasures and secret letters. |
| **Visual Description** | A tall wooden shelf against a textured wall, covered in a thin layer of dust. Shelves hold old books with faded spines, a ceramic sugar bowl, vintage postcards tucked between books, a small brass compass, a folded map half-visible behind a frame, dried flowers in a clay pot, a sepia-toned family photo. Dust motes float in a beam of light from the right. Muted warm palette with accent highlights on key objects. |
| **Dominant Colors** | Dusty Brown (#795548), Parchment (#FFF8E1), Faded Teal (#80CBC4), Warm Amber (#FFB74D), Soft Grey (#BDBDBD) |
| **Mood** | Mysterious, Nostalgic, Quiet, Curious |
| **Focal Point** | The folded map half-hidden behind a frame |
| **Image Grid** | 3×3 = 9 tiles |
| **Resolution** | 1024×1024 px, PNG, < 500 KB |

**AI Art Prompt:**
```
"A tall dusty wooden shelf in an old café, books with faded spines, ceramic sugar bowl, vintage postcards, small brass compass, folded old map partially hidden behind a frame, dried flowers in clay pot, sepia family photo, dust motes in a beam of light from the right, cozy cartoon illustration style, warm muted color palette, front-facing view, soft directional lighting from right, mysterious nostalgic mood, colors: dusty brown, parchment, faded teal, warm amber, soft grey, high quality digital illustration, 1024x1024"
```

#### D. Reveal Mechanics

| Field | Value |
|-------|-------|
| Method | Hybrid |
| Logic | Progressive adjacent reveal |
| Locked Tiles | 1–2 tiles locked; require Level 3+ merge in adjacent cell to unlock |
| VFX | Same as Level 1 + lock-breaking animation for locked tiles |

#### E. Merge Chains

**Chain 1: Coffee Chain** — Same as Level 1, now extends to Level 5:

| Level | Item Name (EN) | Item Name (AR) | Visual Description |
|-------|---------------|----------------|-------------------|
| 1–4 | (Same as Level 1) | | |
| 5 | Coffee Pot | دلّة قهوة | A polished brass Arabic coffee pot (dallah) with ornate engraving |

**Chain 2: Tea Chain** — Same as Level 1, extends to Level 5:

| Level | Item Name (EN) | Item Name (AR) | Visual Description |
|-------|---------------|----------------|-------------------|
| 1–4 | (Same as Level 1) | | |
| 5 | Tea Set | طقم شاي | A complete ornate tea set on a small brass tray |

**Special Item (Introduced):**
- **Dusty Cloth (قطعة قماش):** Appears occasionally; merging two cloths creates a Polishing Rag that can clean a locked tile when placed adjacent

#### F. Rewards

| Achievement | Reward |
|-------------|--------|
| ⭐ 80% | 75 Gold Coins |
| ⭐⭐ 90% | +35 Coins (Total: 110) |
| ⭐⭐⭐ 100% | +75 Coins + 7 Gems (Total: 150 Coins, 7 Gems) |
| Rare Item | "Old Map (Part 1/3)" |
| Shop Unlock | The in-game Shop opens for the first time |
| Passport Stamp | Stamp #2 |

---

### LEVEL 3 — "Street Window" (نافذة الشارع)

#### A. Basic Info

| Field | Value |
|-------|-------|
| Level Number | 3 |
| Difficulty | Medium-Easy |
| Primary Objective | Reveal 80% |
| Full Completion | 100% for 3 Stars |
| Expected Merges | 35–50 |
| Estimated Time | 7–10 minutes |
| Secondary Objectives | Reveal center tile first; Complete under 40 merges |

#### B. Grid Design

| Field | Value |
|-------|-------|
| Grid Size | 6×6 = 36 cells |
| Locked Tiles | 2 |
| Frozen Tiles | 1 (requires 2 adjacent merges: first removes ice, second reveals) |
| Starting Elements | 8 items (6× Level 1, 2× Level 2) |
| Rationale | Larger grid + frozen mechanic provides gentle challenge ramp |

#### C. Hidden Image

| Element | Detail |
|---------|--------|
| **Image Name** | Street Window |
| **Narrative Description** | The café window overlooking the old main street, where Grandma used to sit and watch the world pass by — and dream of traveling it. |
| **Visual Description** | A large ornate window frame looking out onto a charming old town street. Inside the window sill: a small potted cactus, a cup of half-finished coffee, a dog-eared notebook with sketches. Outside: cobblestone street, colorful storefronts, a bicycle leaning against a lamp post, a cat sitting on a distant ledge, a sliver of sunset sky. The window frame has peeling paint and carved details. Warm golden hour light bathes everything. |
| **Dominant Colors** | Sunset Orange (#FF7043), Sky Blue (#4FC3F7), Cobblestone Grey (#90A4AE), Window Green (#66BB6A), Golden Light (#FFF176) |
| **Mood** | Dreamy, Hopeful, Warm, Adventurous |
| **Focal Point** | The notebook with sketches on the sill |
| **Image Grid** | 4×3 = 12 tiles |
| **Resolution** | 1024×1024 px, PNG, < 500 KB |

**AI Art Prompt:**
```
"A large ornate window in an old café looking out onto a charming old town street, window sill with small potted cactus, half-finished coffee cup, dog-eared notebook with pencil sketches, view of cobblestone street, colorful storefronts, bicycle against a lamp post, cat on a distant ledge, sliver of sunset sky, peeling paint on carved window frame, cozy cartoon illustration style, warm golden hour lighting, dreamy hopeful mood, colors: sunset orange, sky blue, cobblestone grey, window green, golden light, high quality digital illustration, 1024x1024"
```

#### D. Reveal Mechanics

| Field | Value |
|-------|-------|
| Locked Tiles | 2 — require Level 3+ adjacent merge |
| Frozen Tiles | 1 — First merge removes ice (becomes locked), second merge reveals |
| Tutorial | Animated hand guides player through frozen tile mechanic |

#### E. Merge Chains

**Chain 1: Coffee Chain** — Levels 1–5 (same as Level 2)

**Chain 2: Tea Chain** — Levels 1–5 (same as Level 2)

**Chain 3: Travel Chain (سلسلة السفر) — NEW**

| Level | Item Name (EN) | Item Name (AR) | Visual Description |
|-------|---------------|----------------|-------------------|
| 1 | Torn Paper | ورقة ممزقة | A small torn piece of yellowed paper |
| 2 | Paper Scraps | قصاصات ورق | Several paper scraps pieced together |
| 3 | Old Letter | رسالة قديمة | A folded letter with faded handwriting and wax seal |
| 4 | Envelope Bundle | حزمة رسائل | A bundle of envelopes tied with ribbon |
| 5 | Treasure Map | خريطة كنز | A detailed hand-drawn world map with marked routes |
| 6 | Explorer's Journal | دفتر المستكشف | A leather-bound journal with maps, sketches, and pressed flowers |

**Special Items:**
- **Ice Pick (معول جليد):** Created from merging two special crystals; used to help break frozen tiles
- **Golden Key (مفتاح ذهبي):** Final reward — unlocks a bonus secret level

#### F. Rewards

| Achievement | Reward |
|-------------|--------|
| ⭐ 80% | 100 Gold Coins |
| ⭐⭐ 90% | +50 Coins (Total: 150) |
| ⭐⭐⭐ 100% | +100 Coins + 10 Gems (Total: 200 Coins, 10 Gems) |
| Rare Item | "Golden Key" (unlocks secret bonus level) |
| Feature Unlock | Traveler's Passport opens (Page 1 free) |
| Passport Stamp | Stamp #3 |

---
