## Section 4: Obstacles & Special Items

### 4.1 Obstacle Types

| Obstacle_ID | Name (EN) | Name (AR) | Appearance | How to Unlock | Introduced At | Visual States |
|-------------|-----------|-----------|------------|---------------|---------------|---------------|
| obs_lock | Locked Tile | مربع مقفل | Tile with lock/chains icon | Merge Level 3+ item in adjacent cell | Level 2 | Locked → Breaking anim → Revealed |
| obs_ice | Frozen Tile | مربع مجمد | Tile covered in transparent ice | 2 adjacent merges (1st removes ice → becomes Locked; 2nd reveals) | Level 3 | Frozen → Cracking → Locked → Revealed |
| obs_keylock | Key Lock Tile | مربع مفتاح | Tile with keyhole icon | Requires special Tool Chain item (e.g., Key) placed on it | Level 5 | Locked(special) → Key insert anim → Revealed |
| obs_dark | Dark Tile | مربع مظلم | Fully black tile | Merge a light item (lantern/candle) in adjacent cell | Level 15 | Dark → Light spreading → Revealed |
| obs_gold | Golden Bonus Tile | مربع ذهبي | Glowing golden tile | Not an obstacle — BONUS; revealed normally | Level 8 | Glowing → Burst → Reward popup |

### 4.2 Obstacle Distribution Across Levels

| Level | Locked | Frozen | Key Lock | Dark | Golden | Notes |
|-------|--------|--------|----------|------|--------|-------|
| 1 | 0 | 0 | 0 | 0 | 0 | Pure tutorial |
| 2 | 1–2 | 0 | 0 | 0 | 0 | Locked intro |
| 3 | 2 | 1 | 0 | 0 | 0 | Frozen intro |
| 4 | 3 | 2 | 0 | 0 | 0 | |
| 5 | 3 | 2 | 1 | 0 | 0 | Key Lock intro + Tools Chain |
| 6 | 4 | 3 | 1 | 0 | 0 | |
| 7 | 5 | 3 | 2 | 0 | 0 | |
| 8 | 5 | 4 | 2 | 0 | 1 | Golden intro |
| 9 | 6 | 4 | 3 | 0 | 1 | |
| 10 | 7 | 5 | 3 | 0 | 2 | **BOSS LEVEL** — Compound obstacles (Frozen+Locked) |
| 11–14 | 7–8 | 5–6 | 3–4 | 0 | 2 | Gradual increase |
| 15 | 8 | 6 | 4 | 1 | 2 | **Dark Tiles intro** |
| 16–19 | 8–9 | 6–7 | 4–5 | 1–2 | 2–3 | |
| 20 | 10 | 7 | 5 | 2 | 3 | **Move limits intro** |
| 21–24 | 10–11 | 7–8 | 5–6 | 2–3 | 3 | |
| 25 | 12 | 8 | 6 | 3 | 3 | **MID-BOSS** |
| 26–29 | 12–13 | 8–9 | 6–7 | 3–4 | 3–4 | |
| 30 | 14 | 10 | 7 | 4 | 4 | **Chain Reactions intro** |
| 31–39 | 14–16 | 10–12 | 7–9 | 4–5 | 4–5 | Advanced combos |
| 40 | 16 | 12 | 9 | 5 | 5 | **Advanced Mechanics** |
| 41–49 | 16–18 | 12–14 | 9–10 | 5–6 | 5–6 | Peak complexity |
| 50 | 18 | 14 | 10 | 6 | 6 | **FINAL BOSS** — All obstacles |

### 4.3 Tools Chain (سلسلة الأدوات)

| Level | Item (EN) | Item (AR) | Visual Description | Use |
|-------|-----------|-----------|-------------------|-----|
| 1 | Fabric Scrap | قطعة قماش | Small beige fabric scrap | Base element |
| 2 | Rope | حبل | Coiled brown rope | Accumulation |
| 3 | Magnifying Glass | عدسة مكبرة | Brass-framed magnifying glass | Reveals Foggy Tiles |
| 4 | Excavation Brush | فرشاة حفريات | Small brush for cleaning artifacts | Cleans Dirty Tiles |
| 5 | Chisel | أزميل | Small chisel with wooden handle | Opens Locked Tiles |
| 6 | Explorer's Bag | حقيبة مستكشف | Leather bag with all tools | Universal tool — opens any obstacle |

### 4.4 Obstacle Tutorial System

| Level | Obstacle Introduced | Tutorial Flow |
|-------|-------------------|---------------|
| 2 | Locked Tile | Hand pointer → "This tile is locked! 🔒" → Guide to merge Level 3 adjacent → "Great! Powerful merges open locks! 💪" |
| 3 | Frozen Tile | "This tile is frozen! ❄️ It needs two merges!" → First merge: "Ice melting..." → Second merge: "Revealed! 🎉" |
| 5 | Key Lock + Tools | "Special tile! Regular merges won't work 🔑" → Show Tools Chain → Guide to complete chain → "Drag the key onto the special tile!" → "Now you can open any special tile! 🌟" |

---

## Section 5: Comprehensive Difficulty Curve

### 5.1 Difficulty Variables

| Variable | Description |
|----------|-------------|
| Grid Size | Increases from 5×5 to 8×8 |
| Image Pieces | From 9 (3×3) to 49 (7×7) |
| Obstacles | Count and variety increase |
| Merge Chain Complexity | Chains grow longer, more concurrent chains |
| Move Limit | Introduced at Level 20, gets tighter |
| Concurrent Chains | From 2 to 6+ simultaneous chains |

### 5.2 Full Difficulty Curve (50 Levels)

| Level | Grid | Image Pieces | Obstacles | Merge Chains | Move Limit | Est. Time |
|-------|------|-------------|-----------|-------------|------------|-----------|
| 1 | 5×5 | 9 (3×3) | None | 2 chains (4 lvl) | ∞ | 3–5 min |
| 2 | 5×6 | 9 (3×3) | 1–2 Locked | 2 chains (5 lvl) | ∞ | 5–7 min |
| 3 | 6×6 | 12 (4×3) | 2L, 1Fr | 3 chains (5 lvl) | ∞ | 7–10 min |
| 4 | 6×6 | 16 (4×4) | 3L, 2Fr | 3 chains (5 lvl) | ∞ | 8–12 min |
| 5 | 6×6 | 16 (4×4) | 3L, 2Fr, 1K | 3 chains + Tools | ∞ | 10–15 min |
| 6 | 6×6 | 16 (4×4) | 4L, 3Fr, 1K | 3 chains (5 lvl) | ∞ | 10–15 min |
| 7 | 7×7 | 16 (4×4) | 5L, 3Fr, 2K | 3 chains (6 lvl) | ∞ | 12–15 min |
| 8 | 7×7 | 25 (5×5) | 5L, 4Fr, 2K, 1G | 4 chains (6 lvl) | ∞ | 12–18 min |
| 9 | 7×7 | 25 (5×5) | 6L, 4Fr, 3K, 1G | 4 chains (6 lvl) | ∞ | 15–18 min |
| **10** | **7×7** | **25 (5×5)** | **7L, 5Fr, 3K, 2G** | **4 chains (6 lvl)** | **∞** | **15–20 min** |
| 11–14 | 7×7 | 25 (5×5) | Gradual increase | 4 chains (6 lvl) | ∞ | 15–20 min |
| **15** | **7×7** | **25 (5×5)** | **+ Dark Tiles** | **4 chains (6 lvl)** | **∞** | **18–22 min** |
| 16–19 | 7×7 | 25–36 | Mixed complex | 4–5 chains | ∞ | 18–25 min |
| **20** | **7×7** | **36 (6×6)** | **Complex mix** | **5 chains (6 lvl)** | **50 moves** | **20–25 min** |
| 21–24 | 7×7 | 36 (6×6) | High complexity | 5 chains | 50 moves | 20–25 min |
| **25** | **8×8** | **36 (6×6)** | **Very complex** | **5 chains (6 lvl)** | **45 moves** | **25–30 min** |
| 26–29 | 8×8 | 36–49 | Very complex | 5–6 chains | 45 moves | 25–30 min |
| **30** | **8×8** | **49 (7×7)** | **+ Chain Reactions** | **5 chains (7 lvl)** | **45 moves** | **25–35 min** |
| 31–39 | 8×8 | 49 (7×7) | Max complexity | 6 chains (7 lvl) | 42 moves | 25–35 min |
| **40** | **8×8** | **49 (7×7)** | **+ Element Evo** | **6 chains (7 lvl)** | **40 moves** | **30–35 min** |
| 41–49 | 8×8 | 49 (7×7) | Peak | 6+ chains (7 lvl) | 40 moves | 30–40 min |
| **50** | **8×8** | **49 (7×7)** | **All types max** | **6+ chains (7 lvl)** | **40 moves** | **30–40 min** |

*Bold = Milestone Levels. L=Locked, Fr=Frozen, K=Key Lock, G=Golden*

### 5.3 Milestone Levels

| Level | Milestone | What Makes It Special | Special Reward |
|-------|-----------|----------------------|----------------|
| 1 | Tutorial | Learn basics — pure teaching | 100 Coins + 5 Gems |
| 5 | First Tools | Tools Chain + Key Lock tiles introduced | 175 Coins + 7 Gems + First Tool |
| 10 | Boss Level | All learned mechanics combined; grand challenge | 300 Coins + 50 Gems + Exclusive Skin |
| 15 | Dark Tiles | New mechanic (light-based reveal) | 425 Coins + 12 Gems + Lantern Item |
| 20 | Move Limits | Strategic constraint introduced | 550 Coins + 15 Gems + "Strategist" Title |
| 25 | Mid-Boss | Major challenge checkpoint | 675 Coins + 100 Gems + Rare Card |
| 30 | Chain Reactions | Auto-cascade merges introduced | 800 Coins + 20 Gems + Bomb Powerup ×5 |
| 40 | Advanced Mechanics | Element Evolution (auto-upgrade over time) | 1050 Coins + 25 Gems + Legendary Skin |
| 50 | Final Boss | Ultimate challenge — all mechanics at max | 1300 Coins + 500 Gems + "Master Explorer" Title + Legendary Skin |

### 5.4 Anti-Frustration Features

| Feature | Details | Availability |
|---------|---------|-------------|
| Free Hints | 3 free hints/day; highlights a mergeable pair | Always |
| Limited Undo | Undo last 3 moves; 5 undos/day | Level 10+ |
| Retry | Restart level with no penalty | Always |
| Watch Ad for Moves | +10 moves after watching rewarded ad | Level 20+ |
| Dynamic Difficulty (DDA) | After 3 consecutive fails: reduce 1 obstacle OR give free powerup | Automatic, silent |

### 5.5 Anti-Boredom Mechanics

| Mechanic | Level Introduced | How It Works |
|----------|-----------------|-------------|
| Combo System | 15+ | 3 merges within 5 seconds → Combo Bonus (1 extra tile revealed) |
| Chain Reaction | 30+ | Certain items auto-merge adjacent matching items on creation ("bomb" effect) |
| Element Evolution | 40+ | Items left on grid for 10+ merges auto-evolve to next level |
| Mini-Games | Every 5 levels | "Jigsaw Rush" — arrange image pieces quickly for bonus rewards |
| Daily Challenges | Always | "Merge 50 items" / "Complete 3 levels" → 50–100 Gems reward |

---

## Section 6: Monetization Strategy

### 6.1 Golden Rules

1. ✅ Game must be fun **without paying** (80% content is free)
2. ✅ Paid content **accelerates progress only** (no exclusive core content)
3. ✅ All ads are **optional** (Rewarded only)
4. ❌ No Pay-to-Win
5. ❌ No annoying Energy System

### 6.2 Revenue Streams

#### A. In-App Purchases (IAPs)

**Currency Packs:**

| Pack | Contents | Price | Value |
|------|----------|-------|-------|
| Starter Pack | 500 Coins + 50 Gems | $0.99 | Great for beginners |
| Small Pack | 1,500 Coins + 100 Gems | $2.99 | Good |
| Medium Pack | 5,000 Coins + 400 Gems | $4.99 | ⭐ Best Value |
| Large Pack | 12,000 Coins + 1,200 Gems | $9.99 | Very Good |
| Mega Pack | 30,000 Coins + 3,500 Gems | $19.99 | For enthusiasts |

**Power-ups & Boosters:**

| Item | Effect | Coin Price | USD Price |
|------|--------|-----------|-----------|
| Fog Clearer | Reveals 3–5 random tiles instantly | 200 Coins | $0.99 |
| Golden Hammer | Opens any locked/frozen tile | 300 Coins | $1.99 |
| Bomb | Reveals all adjacent tiles in radius | 400 Coins | $2.99 |
| Time Extender | +10 moves (Move Limit levels) | 150 Coins | — |
| Vision | 3-second peek at full image | 100 Coins | — |

**Special Bundles:**

| Bundle | Contents | Price | Trigger |
|--------|----------|-------|---------|
| Welcome Bundle | 1000 Coins + 100 Gems + 5 Fog Clearers | $2.99 | First app open (48h limited) |
| Weekend Bundle | 3000 Coins + 300 Gems + 10 Powerups | $4.99 | Every weekend |
| Holiday Bundle | 10,000 Coins + 1000 Gems + Exclusive Skin | $9.99 | Holidays & events |

**Skip Level:** 500 Gems or $2.99 (only after 5+ attempts; confirmation dialog)

#### B. Rewarded Video Ads

**All ads are 100% optional — player chooses when to watch.**

| Situation | Offer | Reward | Daily Max |
|-----------|-------|--------|-----------|
| Stuck on level | "Watch ad to reveal 1 tile" | 1 random tile revealed | 3 ads |
| Before hard level | "Watch ad for strong start" | 3 random Powerups | 2 ads |
| After losing | "Watch ad for retry" | Retry + 5 extra moves | 5 ads |
| In menu (random) | "Watch ad for coins" | 50–100 Coins | 5 ads |
| Daily Gift Box | "Watch ad to open daily chest" | Random rewards | 1 ad |

- **Max ads/day:** 10 total
- **Never between levels** — ads only on player choice

#### C. Traveler's Passport (Battle Pass)

| Track | Price | Rewards |
|-------|-------|---------|
| **Free Pass** | Free | Basic rewards (small coin amounts) |
| **Premium Pass** | $4.99/month | 2× Coins from all levels, bonus Gems, free Powerups, exclusive Skins, ad-free option |

**Example Reward Tiers:**

| Pass Level | Free Track | Premium Track |
|------------|-----------|---------------|
| Level 1 | 50 Coins | 100 Coins + 5 Gems |
| Level 5 | 100 Coins | 200 Coins + 10 Gems + Fog Clearer |
| Level 10 (Boss) | 200 Coins + 10 Gems | 500 Coins + 50 Gems + Exclusive Skin |
| Level 50 (Final) | 1000 Coins + 100 Gems | 3000 Coins + 500 Gems + Legendary Skin + Special Card |

#### D. Cosmetics (Skins)

| Category | Description | Price |
|----------|-------------|-------|
| Element Skins | Different visual themes for merge items | 500–1000 Gems or $2.99–$4.99 |
| Grid Backgrounds | Wood, stone, metal, space textures | 200 Gems or $0.99 |
| Merge Effects | Stars, hearts, fireworks effects | 300 Gems or $1.99 |
| Custom Sounds | Different merge/reveal sounds | 100 Gems or $0.99 |

**All cosmetics are visual only — zero gameplay impact.**

#### E. Photo Album / Passport Collection

| Album Page | Requirements | Or Purchase |
|-----------|-------------|-------------|
| Page 1 | Free | — |
| Page 2 | 500 Gems | $2.99 |
| Page 3 | 750 Gems | $3.99 |
| Page 4 | 1000 Gems | $4.99 |
| Page 5+ | 1000 Gems each | $4.99 |
| **All Pages** | — | **$14.99 (40% off)** |

**Collection Rewards:**
- Complete 1 page (10 images): 500 Coins + 50 Gems + Rare Card
- Complete full album (50 images): "Master Explorer" 🏆 title + Exclusive Skin + massive coin reward

### 6.3 Balance & Fairness

**Free Player Can:**
- ✅ Complete all levels (may take longer)
- ✅ Earn Coins & Gems through play
- ✅ Unlock album pages via earned Gems
- ✅ Get limited Powerups via rewarded ads

**Paying Player Gets:**
- ⚡ Faster progress (2× Coins)
- ⚡ More convenience (more Powerups)
- ⚡ Exclusive cosmetics (Skins)
- ❌ No exclusive story content or exclusive levels

### 6.4 Strategic Sales Points

| Trigger | What We Show | Rules |
|---------|-------------|-------|
| After Level 3 (first time) | Premium Pass offer | Dismissible; one-time |
| After 3 fails on same level | Powerup offer | Only after 3 real fails |
| At Milestone Levels (5, 10, 15…) | Starter Pack offer | Optional |
| Player-initiated | In-game Shop (always accessible) | No pop-ups |

**Never Do:**
- ❌ Forced pop-ups after every level
- ❌ Offers during gameplay
- ❌ Tiny/hidden close buttons
- ❌ Reducing fun to force payment

### 6.5 Revenue Projections

| Scenario | Players | Conversion Rate | ARPPU | Monthly Revenue |
|----------|---------|----------------|-------|----------------|
| Conservative (Month 1) | 10,000 | 2–3% | $5–10 | $1,000–3,000 |
| Moderate (Month 6) | 50,000 | 3–5% | $10–15 | $15,000–37,500 |
| Optimistic (Year 1) | 200,000 | 5–7% | $15–20 | $150,000–280,000 |

---

## Section 7: Internal Economy

### 7.1 Currency Types

#### A. Soft Currency — Gold Coins 💰

**Characteristics:** Earned abundantly from play; spent on consumables; fast cycle (Earn → Spend → Earn)

**Earning Methods:**

| Method | Amount | Frequency | Notes |
|--------|--------|-----------|-------|
| Complete Level (80%) | 50 + (Level × 25) Coins | Per level | Primary source |
| Complete Level (100%) | Double the above | On mastery | Mastery bonus |
| Daily Chest | 20–100 random Coins | 1×/day | Return incentive |
| Daily Login Streak | 10 × (consecutive days) | Daily | Resets after 7 days |
| Watch Optional Ad | 50 Coins | Up to 5×/day | Max 250 Coins/day |
| Achievement | 100–500 Coins | On completion | One-time |
| Replay Level | 50% of original reward | Unlimited | Secondary source |
| Sell Items | By item level | On sell | Secondary |

**Spending Methods:**

| Item | Price | Effect | Type |
|------|-------|--------|------|
| Fog Clearer | 200 Coins | Reveals 3–5 random tiles | Consumable |
| Hint | 100 Coins | Highlights mergeable pair | Consumable |
| Golden Hammer | 300 Coins | Opens any locked/frozen tile | Consumable |
| Bomb | 400 Coins | Reveals all tiles in radius | Consumable |
| Skip Cooldown | 50 Coins | Instant new element from generator | Repeatable |
| Time Extender | 150 Coins | +10 moves | Move Limit levels |
| Retry with Bonus | 150 Coins | Restart + 3 random powerups | On fail |
| Buy Specific Item | 100–500 Coins | Place specific level item on grid | Advanced |

**Earn vs Spend Balance:**
```
Average Earning per level (100%): ~125 Coins
Average Powerup cost: 200–300 Coins
→ Player needs 2–3 levels to afford 1 powerup (ideal balance)
```

#### B. Hard Currency — Gems 💎

**Characteristics:** Rare; hard to earn from play; spent on permanent/high-value items; primary source is real money

**Earning Methods:**

| Method | Amount | Frequency | Notes |
|--------|--------|-----------|-------|
| Complete Level (100% only) | 5 + (Level ÷ 2) Gems | On mastery only | Very few |
| Major Achievement | 20–100 Gems | On achievement | e.g., "Complete 10 levels" |
| Daily Challenge | 10–30 Gems | Daily (if completed) | Relatively hard |
| Weekly Challenge | 50–100 Gems | Weekly | Harder |
| Boss/Milestone Levels | 50–100 Gems | Level 10, 20, 30… | Special reward |
| Watch Ads (limited) | 5 Gems | 1–2×/day max | Very limited |
| Purchase with money | 100–3,500 Gems | Per pack | Primary source |

**Spending Methods:**

| Item | Price | Effect | Type |
|------|-------|--------|------|
| Album Page Unlock | 500–1000 Gems | New page (10 images) | Permanent |
| Exclusive Skin | 200–500 Gems | Change item/background appearance | Permanent |
| Convert to Coins | 100 Gems = 1000 Coins | Emergency coin source | Consumable |
| Skip Level | 500 Gems | Complete current level | Rare use |
| Premium Pass | $4.99 (real money) | Monthly subscription rewards | Subscription |
| Exclusive Content Pack | 1000–2000 Gems | Exclusive themes/landmarks | Content |

**Earn vs Spend Balance:**
```
Average Earning per level (100%): ~7 Gems
Album page cost: 500–1000 Gems
→ 70–140 perfect levels to unlock 1 page
→ OR pay $4.99 for instant unlock
→ Creates strong but fair monetization incentive
```

### 7.2 Economy Flow in First 3 Levels

#### Level 1: Introduction

| Phase | Details |
|-------|---------|
| **Starting Balance** | 0 Coins, 0 Gems |
| **During Level** | No shop, no powerups — pure tutorial |
| **80% Completion** | +50 Coins |
| **90% Completion** | +25 Coins (Total: 75) |
| **100% Completion** | +50 Coins + 5 Gems (Total: 100 Coins, 5 Gems) |
| **Shop Status** | Not yet unlocked |
| **Economic Goal** | Introduce coins; create achievement feeling |

#### Level 2: Creating Need

| Phase | Details |
|-------|---------|
| **Starting Balance** | 100 Coins, 5 Gems |
| **During Level** | Slightly harder (1–2 locked); player may struggle |
| **100% Completion** | +150 Coins + 7 Gems (Total: 250 Coins, 12 Gems) |
| **Shop Opens** | Fog Clearer (200), Hint (100), Golden Hammer (300, insufficient funds), Bomb (locked until L5) |
| **Economic Goal** | Teach spending; create Need + Scarcity |

#### Level 3: Creating Loop

| Phase | Details |
|-------|---------|
| **Starting Balance** | 250 Coins, 12 Gems (if no purchase) OR 50 Coins, 12 Gems (if bought Fog Clearer) |
| **During Level** | Harder (2 Locked + 1 Frozen); powerup helps if purchased |
| **100% Completion** | +200 Coins + 10 Gems |
| **Passport Opens** | First page free; next pages cost 500 Gems |
| **Economic Goal** | Complete the Earn→Spend→Earn loop; introduce long-term goal (Gem collection) |

#### Player Balance After 3 Levels (Scenarios)

| Scenario | Coins | Gems | Status |
|----------|-------|------|--------|
| A: Cautious (no purchases) | 450 | 22 | Good balance; will need to buy soon at L4–5 |
| B: Moderate (1 Fog Clearer) | 250 | 22 | Excellent balance |
| C: Heavy spender (2 powerups) | 150 | 22 | Feeling pressure → IAP opportunity |

### 7.3 Economy Balance Formulas

**Coin Earning Formula:**
```
Coins_Per_Level(100%) = 50 + (Level × 25)

Examples:
Level 1:  75 Coins (given 100 as first-level bonus)
Level 5:  175 Coins
Level 10: 300 Coins
Level 20: 550 Coins
Level 50: 1,300 Coins
```

**Gem Earning Formula:**
```
Gems_Per_Level(100%) = 5 + (Level ÷ 2)

Examples:
Level 1:  5 Gems
Level 5:  7 Gems
Level 10: 10 Gems
Level 20: 15 Gems
Level 50: 30 Gems
```

**Gem Accumulation Rate (Free Player):**
```
20 levels (100%): ~140 Gems
+ Daily Login (7 days × 20): +140 Gems
+ Achievement bonuses: +50 Gems
+ Daily Challenges (7 × 20): +140 Gems
+ Boss Levels: +100 Gems
= ~570 Gems in ~20 days
→ 1 album page every 3–4 weeks of regular play
```

### 7.4 Economy Metrics to Track

| Metric | Target | If Lower | If Higher |
|--------|--------|----------|-----------|
| Coins/Day (Active User) | 300–500 | Earning too slow → frustration | Earning too fast → no pay incentive |
| Gems/Day (Active User) | 20–40 | Too slow → won't unlock pages | Too fast → no pay incentive |
| Powerup Usage Rate | 30–50% of levels | Players don't see value | Levels too hard |
| Coin Sink Rate | 60–80% of earnings | Hoarding → raise powerup prices | Over-spending → coin shortage |
| Conversion Rate (IAP) | 2–5% | Free system working (normal) | Strong pay incentive |
| Time to First IAP | 3–7 days | Economy balanced | Too early pressure → bad |

---
