namespace TNRD.Modkist.Models;

public record DependencyModel(uint ModId, long Stamp, HashSet<uint> Dependencies);
