public interface IControllable
{
    public void SetInput(float speed, float turn);
    public void Brake();
    public float GetSpeed();
}