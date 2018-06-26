using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class optionsmenuscript : MonoBehaviour
{
    public GUISkin skin;
    public Rect windowRect;
    public int menu_width;
    public int menu_height;
    public bool show_menu;
    public void OnApplicationPause()
    {
        Screen.lockCursor = false;
    }

    public void OnApplicationFocus()
    {
        if (!this.show_menu)
        {
            Screen.lockCursor = true;
        }
    }

    public void ShowMenu()
    {
        this.show_menu = true;
    }

    public void HideMenu()
    {
        this.show_menu = false;
    }

    public void OnGUI()
    {
        if (!this.show_menu)
        {
            return;
        }
        this.windowRect = GUI.Window(0, new Rect((Screen.width * 0.5f) - (this.menu_width * 0.5f), (Screen.height * 0.5f) - (this.menu_height * 0.5f), this.menu_width, this.menu_height), this.WindowFunction, "", this.skin.window);
    }

    private Vector2 draw_cursor;
    private Vector2 draw_cursor_line;
    private int line_height;
    private int line_offset;
    public void DrawCursor_Init()
    {
        this.draw_cursor = new Vector2(25, 25);
        this.draw_cursor_line = new Vector2(0, 0);
    }

    public void DrawCursor_NextLine()
    {
        this.draw_cursor_line = new Vector2(0, 0);
        this.draw_cursor.y = this.draw_cursor.y + this.line_offset;
    }

    public void DrawCursor_Offset(float val)
    {
        this.draw_cursor_line.x = this.draw_cursor_line.x + val;
    }

    public Vector2 DrawCursor_Get()
    {
        return this.draw_cursor + this.draw_cursor_line;
    }

    public Rect DrawCursor_RectSpace(float width)
    {
        Rect rect = new Rect(this.draw_cursor.x + this.draw_cursor_line.x, this.draw_cursor.y + this.draw_cursor_line.y, width, this.line_height);
        this.DrawCursor_Offset(width);
        return rect;
    }

    public void DrawLabel(string text)
    {
        this.DrawCursor_Offset(17);
        GUI.Label(this.DrawCursor_RectSpace(400), text, this.skin.label);//GUI.skin.label.CalcSize(new GUIContent(text)).x),
    }

    public bool DrawCheckbox(bool val, string text)
    {
        val = GUI.Toggle(this.DrawCursor_RectSpace(400), val, text, this.skin.toggle);
        return val;
    }

    public float DrawSlider(float val)
    {
        this.DrawCursor_Offset(18);
        val = GUI.HorizontalSlider(this.DrawCursor_RectSpace(400 - this.draw_cursor_line.x), val, 0f, 1f);
        return val;
    }

    public bool DrawButton(string text)
    {
        bool val = GUI.Button(this.DrawCursor_RectSpace(200), text, this.skin.button);
        return val;
    }

    //private var brightness = 0.3;
    private float master_volume;
    private float sound_volume;
    private float music_volume;
    private float voice_volume;
    private bool lock_gun_to_center;
    private bool mouse_invert;
    private float mouse_sensitivity;
    private bool show_advanced_sound;
    private bool toggle_crouch;
    private Vector2 scroll_view_vector;
    private float vert_scroll;
    private float vert_scroll_pixels;
    private float gun_distance;
    public void RestoreDefaults()
    {
        this.master_volume = 1f;
        this.sound_volume = 1f;
        this.music_volume = 1f;
        this.voice_volume = 1f;
        this.mouse_sensitivity = 0.2f;
        this.lock_gun_to_center = false;
        this.mouse_invert = false;
        this.toggle_crouch = true;
    }

    public void Start()
    {
        Screen.lockCursor = true;
        this.RestoreDefaults();
        this.master_volume = PlayerPrefs.GetFloat("master_volume", this.master_volume);
        this.sound_volume = PlayerPrefs.GetFloat("sound_volume", this.sound_volume);
        this.music_volume = PlayerPrefs.GetFloat("music_volume", this.music_volume);
        this.voice_volume = PlayerPrefs.GetFloat("voice_volume", this.voice_volume);
        this.mouse_sensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", this.mouse_sensitivity);
        this.lock_gun_to_center = PlayerPrefs.GetInt("lock_gun_to_center", this.lock_gun_to_center ? 1 : 0) == 1;
        this.mouse_invert = PlayerPrefs.GetInt("mouse_invert", this.mouse_invert ? 1 : 0) == 1;
        this.toggle_crouch = PlayerPrefs.GetInt("toggle_crouch", this.toggle_crouch ? 1 : 0) == 1;
        this.gun_distance = PlayerPrefs.GetFloat("gun_distance", 1f);
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("master_volume", this.master_volume);
        PlayerPrefs.SetFloat("sound_volume", this.sound_volume);
        PlayerPrefs.SetFloat("music_volume", this.music_volume);
        PlayerPrefs.SetFloat("voice_volume", this.voice_volume);
        PlayerPrefs.SetFloat("mouse_sensitivity", this.mouse_sensitivity);
        PlayerPrefs.SetInt("lock_gun_to_center", this.lock_gun_to_center ? 1 : 0);
        PlayerPrefs.SetInt("mouse_invert", this.mouse_invert ? 1 : 0);
        PlayerPrefs.SetInt("toggle_crouch", this.toggle_crouch ? 1 : 0);
        PlayerPrefs.SetFloat("gun_distance", this.gun_distance);
    }

    public bool IsMenuShown()
    {
        return this.show_menu;
    }

    public void Update()
    {
        if (this.vert_scroll != -1f)
        {
            this.vert_scroll = this.vert_scroll + Input.GetAxis("Mouse ScrollWheel");
        }
        if (Input.GetKeyDown("escape") && !this.show_menu)
        {
            Screen.lockCursor = false;
            this.ShowMenu();
        }
        else
        {
            if (Input.GetKeyDown("escape") && this.show_menu)
            {
                Screen.lockCursor = true;
                this.HideMenu();
            }
        }
        if (Input.GetMouseButtonDown(0) && !this.show_menu)
        {
            Screen.lockCursor = true;
        }
        if (this.show_menu)
        {
            Time.timeScale = 0f;
        }
        else
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void WindowFunction(int windowID)
    {
        this.scroll_view_vector = GUI.BeginScrollView(new Rect(0, 0, this.windowRect.width, this.windowRect.height), this.scroll_view_vector, new Rect(0, this.vert_scroll_pixels, this.windowRect.width, this.windowRect.height));
        this.DrawCursor_Init();
        this.mouse_invert = this.DrawCheckbox(this.mouse_invert, "Invert mouse");
        this.DrawCursor_NextLine();
        this.DrawLabel("Mouse sensitivity:");
        this.DrawCursor_NextLine();
        this.mouse_sensitivity = this.DrawSlider(this.mouse_sensitivity);
        this.DrawCursor_NextLine();
        this.DrawLabel("Distance from eye to gun:");
        this.DrawCursor_NextLine();
        this.gun_distance = this.DrawSlider(this.gun_distance);
        this.DrawCursor_NextLine();
        this.toggle_crouch = this.DrawCheckbox(this.toggle_crouch, "Toggle crouch");
        this.DrawCursor_NextLine();
        this.lock_gun_to_center = this.DrawCheckbox(this.lock_gun_to_center, "Lock gun to screen center");
        this.DrawCursor_NextLine();
        /*DrawLabel("Brightness:");
	DrawCursor_NextLine();
	brightness = DrawSlider(brightness);
	DrawCursor_NextLine();*/
        this.DrawLabel("Master volume:");
        this.DrawCursor_NextLine();
        this.master_volume = this.DrawSlider(this.master_volume);
        this.DrawCursor_NextLine();
        this.show_advanced_sound = this.DrawCheckbox(this.show_advanced_sound, "Advanced sound options");
        this.DrawCursor_NextLine();
        if (this.show_advanced_sound)
        {
            int indent = 44;
            this.DrawLabel("....Sounds:");
            this.DrawCursor_NextLine();
            this.DrawCursor_Offset(indent);
            this.sound_volume = this.DrawSlider(this.sound_volume);
            this.DrawCursor_NextLine();
            this.DrawLabel("....Voice:");
            this.DrawCursor_NextLine();
            this.DrawCursor_Offset(indent);
            this.voice_volume = this.DrawSlider(this.voice_volume);
            this.DrawCursor_NextLine();
            this.DrawLabel("....Music:");
            this.DrawCursor_NextLine();
            this.DrawCursor_Offset(indent);
            this.music_volume = this.DrawSlider(this.music_volume);
            this.DrawCursor_NextLine();
        }
        if (this.DrawButton("Resume"))
        {
            Screen.lockCursor = true;
            this.show_menu = false;
        }
        this.draw_cursor.y = this.draw_cursor.y + (this.line_offset * 0.3f);
        this.DrawCursor_NextLine();
        if (this.DrawButton("Restore defaults"))
        {
            this.RestoreDefaults();
        }
        this.DrawCursor_NextLine();
        this.draw_cursor.y = this.draw_cursor.y + (this.line_offset * 0.3f);
        if (this.DrawButton("Exit game"))
        {
            Application.Quit();
        }
        float content_height = this.draw_cursor.y;
        GUI.EndScrollView();
        if (content_height > this.windowRect.height)
        {
            float leeway = (content_height / this.windowRect.height) - (this.windowRect.height / content_height);
            if (this.vert_scroll == -1f)
            {
                this.vert_scroll = leeway;
            }
            this.vert_scroll = GUI.VerticalScrollbar(new Rect(this.menu_width - 20, 20, this.menu_width, this.menu_height - 25), this.vert_scroll, this.windowRect.height / content_height, content_height / this.windowRect.height, 0f);
            this.vert_scroll_pixels = this.windowRect.height * (leeway - this.vert_scroll);
        }
        else
        {
            this.vert_scroll = -1f;
            this.vert_scroll_pixels = 0f;
        }
        this.SavePrefs();
    }

    public optionsmenuscript()
    {
        this.menu_width = 300;
        this.menu_height = 500;
        this.line_height = 24;
        this.line_offset = 24;
        this.master_volume = 1f;
        this.sound_volume = 1f;
        this.music_volume = 1f;
        this.voice_volume = 1f;
        this.mouse_sensitivity = 0.2f;
        this.toggle_crouch = true;
        this.scroll_view_vector = new Vector2(0, 0);
        this.gun_distance = 1f;
    }

}