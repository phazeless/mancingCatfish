using System;

public interface ISwimBehaviour : ICloneable
{
	void SetSwimPatternMono(SwimBehaviourMono swimPatternMono);

	void Awake();

	void Start();

	void Update();

	void FixedUpdate();
}
