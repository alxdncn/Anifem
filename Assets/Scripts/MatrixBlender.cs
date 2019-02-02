using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof(Camera))]
public class MatrixBlender : MonoBehaviour
{
	private Matrix4x4   ortho,
                        perspective;
    public float        fov     = 60f,
                        near    = .3f,
                        far     = 1000f,
                        orthographicSize = 5f;
    private float       aspect;
    private MatrixBlender blender;
    private bool        orthoOn;

	[SerializeField] float lerpTime = 5f;
 
	Camera camera;

    void Start()
    {
		camera = GetComponent<Camera>();
        aspect = (float) Screen.width / (float) Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        camera.projectionMatrix = ortho;
        orthoOn = true;
        blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));
    }
 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            orthoOn = !orthoOn;
            if (orthoOn)
                blender.BlendToMatrix(ortho, lerpTime);
            else
                blender.BlendToMatrix(perspective, lerpTime);
        }
    }

    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }
 
    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            camera.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration);
            yield return 1;
        }
        camera.projectionMatrix = dest;
    }
 
    public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(camera.projectionMatrix, targetMatrix, duration));
    }
}