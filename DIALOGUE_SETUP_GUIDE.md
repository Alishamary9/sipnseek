# 🛠️ Sip & Seek: Dialogue System Setup Guide

This guide will help you create your first dialogue scene in 5 minutes.

## Step 1: generating the System (Automated)
1.  Open your scene in Unity.
2.  In the top menu, click **Tools** > **Sip & Seek** > **Create Dialogue UI**.
    *   *Result:* This creates a `DialogueCanvas` (UI) and a `Managers` object.

## Step 2: Fix "GameDatabase" Warning (One-Time)
1.  If you see a yellow warning `GameDatabase not found`:
2.  Go to menu: **Tools** > **Sip & Seek** > **Generate All Data**.
3.  *Result:* The warning should disappear.

## Step 3: Create the Dialogue Graph (The Fun Part!)
1.  Create a new Empty GameObject in the scene. Name it **"Conversations"**.
2.  Add Component: **Dialogue Trigger**.
3.  Add Component: **FSM Owner** (This is from NodeCanvas).
4.  In the **FSM Owner** component:
    *   Click the **"Bound"** button (or "Create New").
    *   Click **"Edit"** to open the graph window.

## Step 4: Building the Conversation
In the NodeCanvas Window:

1.  **Right Click** > **Add Node** > **Action State**.
2.  Select the new node. In the Inspector (on the right):
    *   Click **"Add Action"**.
    *   Search for **"Sip & Seek"** category.

3.  **Add these 3 Actions in order:**
    *   1️⃣ `Start Dialogue Mode` (Pauses game)
    *   2️⃣ `Show Dialogue`
        *   **Character Name:** `Laith`
        *   **Emotion:** `Normal`
        *   **Text Key:** `dlg_lv1_pre_1`
    *   3️⃣ `Show Dialogue` (Add another one for the Grandma)
        *   **Character Name:** `Grandma`
        *   **Emotion:** `Happy`
        *   **Text Key:** `dlg_lv1_pre_2`
    *   4️⃣ `End Dialogue Mode` (Resumes game)

## Step 5: Play!
1.  Press **Play** in Unity.
2.  The dialogue should appear immediately (because `DialogueTrigger` starts automatically).

---

### 🔑 Useful Keys for Testing
Copy-paste these keys into the "Text Key" field:

| Key | Text (English) | النص (العربية) |
|-----|----------------|----------------|
| `dlg_lv1_pre_1` | Grandma, why do you sit here? | جدة، ليش كل مرة بتقعدي هون؟ |
| `dlg_lv1_pre_2` | Come sit with me... | تعال يا حبيبي، اقعد جنبي... |
| `dlg_lv2_post_1` | Grandma! Old photos... | جدة! صور قديمة... ورسايل! |
