using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DEBUG

public class DebugMenu : MonoBehaviour
{
    public static DebugMenu instance = null; //singleton instance

    struct MenuAction
    {
        public KeyCode key;
        public string name;
    }

    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    private List<MenuAction> menuActions = new List<MenuAction>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddAction(string name, Action action)
    {
        actions.Add(name, action);
        KeyCode key = (KeyCode.Keypad0 + menuActions.Count);
        menuActions.Add(new MenuAction { key = key, name = name });
    }

    private void Update()
    {
        foreach (var ma in menuActions)
        {
            if (Input.GetKeyDown(ma.key))
            {
                Action action = actions[ma.name];
                action();
            }
        }
    }

    void OnGUI()
    {
        Rect r = new Rect(0f, 0f, 200f, 100f);
        foreach (var ma in menuActions)
        {
            GUI.Label(r, $"[{ma.key}] {ma.name}");
            r.y += 10f;
        }
    }
}

#endif
