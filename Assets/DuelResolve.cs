using System.Text;
public class DuelResolve
{
    public static DuelResolve[] GetResolvesTable()
    {
        DuelResolve[] ret = new DuelResolve[Range];
        for (int i = 0; i < Range; i++)
        {
            ret[i] = new DuelResolve(i + 1);
        }
        return ret;
    }

    public const int Range = 40;
    private readonly int _baseVal;
    private DuelOutcome[] _outcomes;
    public DuelOutcome[] Outcomes{ get{ return _outcomes; } }

    public DuelResolve(int baseVal)
    {
        _baseVal = baseVal;
        _outcomes = new DuelOutcome[Range];
        for (int i = 0; i < Range; i++)
        {
            _outcomes[i] = GetOutcome(baseVal, i + 1);
        }
    }
    public static DuelOutcome GetOutcome(int fistTarget, int secondTarget)
    {
        DuelResult result = Resolve(fistTarget, secondTarget);
        return new DuelOutcome(fistTarget, secondTarget, result.Remaining, result.FirstSurvives);
    }

    private struct DuelResult
    {
        public int Remaining;
        public bool FirstSurvives;
        public DuelResult(int remaining, bool firstSurvives)
        {
            Remaining = remaining;
            FirstSurvives = firstSurvives;
        }
    }

    private static DuelResult Resolve(int firstTarget, int secondTarget)
    {
        firstTarget -= secondTarget;
        if (firstTarget > 0)
        {
            return ResolveBack(secondTarget, firstTarget);
        }
        else
        {
            return new DuelResult(secondTarget, true);
        }
    }

    private static DuelResult ResolveBack(int secondTarget, int firstTarget)
    {
        secondTarget -= firstTarget;
        if (secondTarget > 0)
        {
            return Resolve(firstTarget, secondTarget);
        }
        else
        {
            return new DuelResult(firstTarget, false);
        }
    }
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        foreach (DuelOutcome item in _outcomes)
        {
            builder.Append(item.Damage + " ");
        }
        return builder.ToString();
    }
}
