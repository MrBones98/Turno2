using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectButton
{
    public Button Button;
    public Level Level;
    public int LevelNumber;

    public LevelSelectButton(Level level, int lvlNumber, VisualTreeAsset template)
    {
        TemplateContainer templateContainer = template.Instantiate();

        Button = templateContainer.Q<Button>();

        this.Level = level;
        this.LevelNumber = lvlNumber;

        Button.name = $"{Level.Name}Button";
        Button.text = $"{lvlNumber} - {Level.Name}";

        Debug.Log($"Button {LevelNumber} - {Level.Name} Instantiated");
    }
}
