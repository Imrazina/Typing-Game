namespace GameLibrary;

// TODO: Vytvořte třídu UpdatedStatsEventArgs, která je potomkem EventArgs
public class UpdatedStatsEventArgs : EventArgs
{
    public int Correct { get; init; }
    public int Missed { get; init; }
    public int Accuracy { get; init; }
}

// TODO: Vytvořte delegát UpdatedStatsEventHandler pro příslušnou událost, využijte výše definované argumenty

public delegate void UpdatedStatsEventHandler(object sender, UpdatedStatsEventArgs e);

// TODO: Dokončete třídu Stats...
public class Stats
{
    // TODO: Vytvořte vlastnosti určené pro čtení:
    private int correct;
    private int missed;

    public int Correct => correct;
    public int Missed => missed;
    public int Accuracy => (int)Math.Round(100.0 * Correct / (Correct + Missed));



    // TODO: Vytvořte veřejnou událost UpdatedStats (UpdatedStatsEventHandler?)

    public event UpdatedStatsEventHandler? UpdatedStats;
    
    // TODO: Vytvořte veřejnou metodu Update
    public void Update(bool correctKey)
    {
        if (correctKey)
            correct++;
        else
            missed++;

        UpdatedStats?.Invoke(this, new UpdatedStatsEventArgs
        {
            Correct = this.Correct,
            Missed = this.Missed,
            Accuracy = this.Accuracy
        });
    }
    // TODO: Vytvořte veřejnou metodu Reset
    public void Reset()
    {
        correct = 0;
        missed = 0;
    }
}
