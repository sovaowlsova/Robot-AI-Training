using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecordScript : MonoBehaviour
{
    private float verticalInput;
    private float horizontalInput;
    private List<string> commands;
    bool recording = false;
    float delayTime = 0.0f;

    bool moving = false;
    bool braking = false;
    int turn = 0;

    private InputAction moveAction;
    private InputAction brakeAction;
    private InputAction recordAction;

    public bool isRecording()
    {
        return recording;
    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        brakeAction = InputSystem.actions.FindAction("Jump");
        recordAction = InputSystem.actions.FindAction("Record");
    }

    // Update is called once per frame
    void Update()
    {
        if (recordAction.WasPressedThisFrame())
        {
            recording = !recording;
            if (recording)
            {
                commands = new List<string>();
                commands.Add("enable");
            } else
            {
                commands.Add("disable");
                saveToCSV();
                commands.Clear();
                commands = null;
            }
        }

        if (!recording)
        {
            return;
        }

        List<string> actions = getActions();
        if (actions.Count > 0)
        {
            if (delayTime > 0.0f)
            {
                commands.Add("delay");
                commands.Add(((int)(delayTime * 1000)).ToString());
                delayTime = 0.0f;
            }
            commands.AddRange(actions);
        } else
        {
            delayTime += Time.deltaTime;
        }
    }

    List<string> getActions()
    {
        Vector2 moveResult = moveAction.ReadValue<Vector2>();
        verticalInput = moveResult.y;
        horizontalInput = moveResult.x;
        List<string> actions = new List<string>();

        if (horizontalInput > 0 && turn != 1)
        {
            turn = 1;
            actions.Add("turn_right");
        }
        else if (horizontalInput < 0 && turn != -1)
        {
            turn = -1;
            actions.Add("turn_left");
        }
        else if (horizontalInput == 0 && turn != 0)
        {
            turn = 0;
            actions.Add("turn_straight");
        }

        bool brakingAction = brakeAction.ReadValue<float>() > 0;

        if (brakingAction)
        {
            if (!braking)
            {
                actions.Add("brakes_on");
                braking = true;
                moving = false;
            }
            return actions;
        }
        else if (braking)
        {
            braking = false;
            actions.Add("brakes_off");
        }

        if (verticalInput > 0 && !moving)
        {
            moving = true;
            actions.Add("go_forwards");
        } else if (verticalInput < 0 && !moving)
        {
            moving = true;
            actions.Add("go_backwards");
        } else if (verticalInput == 0 && moving && !braking)
        {
            moving = false;
            actions.Add("stop");
        }

        return actions;
    }

    private void saveToCSV()
    {
        if (commands == null || commands.Count == 0)
        {
            Debug.Log("Not commands in list or list does not exist.");
            return;
        }

        string logsFolder = System.IO.Path.Combine(Application.persistentDataPath, "Logs");
        if (!System.IO.Directory.Exists(logsFolder))
        {
            System.IO.Directory.CreateDirectory(logsFolder);
        }

        string logFilePath = System.IO.Path.Combine(logsFolder, $"log_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv");
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, $"active_command.csv");
        string commandString = string.Join(",", commands.ToArray());
        System.IO.File.WriteAllText(logFilePath, commandString);
        System.IO.File.WriteAllText(filePath, commandString);
        Debug.Log("Log written to " + logFilePath);

        string[] oldLogs = System.IO.Directory.GetFiles(logsFolder, "log_*.csv");
        if (oldLogs.Length > 20)
        {
            System.Array.Sort(oldLogs);
            System.IO.File.Delete(oldLogs[0]);
        }
    }
}