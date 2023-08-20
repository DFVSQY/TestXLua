
public class PData
{
    public int score { get; set; }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int value)
    {
        score = value;
    }
}

public class Person
{
    private int _age;
    public int age
    {
        get
        {
            return _age;
        }
        set
        {
            _age = value;
        }
    }

    public string name { get; set; }

    public PData data { get; } = new PData();

    public void SetName(string name)
    {
        this.name = name;
    }

    public string GetName()
    {
        return name;
    }
}