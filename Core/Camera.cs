using System.Numerics;

namespace Riateu;

public class Camera 
{
    private Matrix4x4 projection;
    private Matrix4x4 viewMatrix;
    private Vector2 position;

    public Camera(Vector2 position) 
    {
        this.position = position;
        ChangeProjection();
    }

    public void ChangeProjection() 
    {
        projection = Matrix4x4.Identity;
        projection = Matrix4x4.CreateOrthographicOffCenter(0.0f, 32.0f * 40.0f, 0.0f, 32.0f * 21.0f, 0.0f, 100f);
    }

    public Matrix4x4 GetViewMatrix() 
    {
        var cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        var cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
        viewMatrix = Matrix4x4.Identity;
        viewMatrix = Matrix4x4.CreateLookAt(
            new Vector3(position.X, position.Y, 20.0f), 
            cameraFront + new Vector3(position.X, position.Y, 0.0f),
            cameraUp
        );
        return viewMatrix;
    }

    public Matrix4x4 GetProjectionMatrix() => projection;
}