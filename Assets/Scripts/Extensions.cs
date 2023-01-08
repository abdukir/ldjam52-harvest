using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Remap a value to given range
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="newMin"></param>
    /// <param name="newMax"></param>
    /// <returns></returns>
    public static float Remap(this float value, float min, float max, float newMin, float newMax)
    {
        if (Mathf.Approximately(max, min))
        {
            return value;
        }

        return newMin + (value - min) * (newMax - newMin) / (max - min);
    }

    /// <summary>
    /// Remap a value to 0-1 range
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float Remap01(this float value, float min, float max)
    {
        return value.Remap(min, max, 0f, 1f);
    }

    /// <summary>
    /// Convert RenderTexture to Texture2D
    /// </summary>
    /// <param name="rTex"></param>
    /// <returns></returns>
    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }

    /// <summary>
    /// Take a "screenshot" of a camera's Render Texture.
    /// </summary>
    /// <param name="camera"></param>
    /// <returns>s</returns>
    public static Texture2D RTImage(this Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }

    /// <summary>
    /// Set the parent of the GameObject without caching it. Also destroys it if you give destroy time a value
    /// </summary>
    /// <param name="Parent"></param>
    /// <returns></returns>
    public static GameObject ParentSetAndDestroy(this GameObject _obj, Transform _parent, float _destroyTime = 0f)
    {
        _obj.transform.SetParent(_parent);
        if (_destroyTime > 0)
        {
            MonoBehaviour.Destroy(_obj, _destroyTime);
        }
        return _obj;
    }

    /// <summary>
    /// Get the center point of the Transforms in the given list
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetCenterPoint(this List<Transform> targets)
	{
		if (targets.Count == 1)
		{
            return targets[0].position;
		}

        var bounds = new Bounds(targets[0].position, Vector3.zero);
		for (int i = 0; i < targets.Count; i++)
		{
            bounds.Encapsulate(targets[i].position);
		}

        return bounds.center;

	}

    /// <summary>
    /// Get a random integer except the given one
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="except"></param>
    /// <returns></returns>
    public static int RandomExcept(int min, int max, int except)
    {
        int random = Random.Range(min, max);
        if (random >= except) random = (random + 1) % max;
        return random;
    }

    /// <summary>
    /// Return height of an object based on its collider
    /// </summary>
    /// <param name="_obj"></param>
    /// <returns></returns>
    public static float GetColliderHeight(this Transform _obj)
	{
        return _obj.GetComponent<Collider>().bounds.extents.y * 2;
	}

    /// <summary>
    /// Return height of an object based on its collider
    /// </summary>
    /// <param name="_obj"></param>
    /// <returns></returns>
    public static float GetColliderWidth(this Transform _obj)
    {
        return _obj.GetComponent<Collider>().bounds.extents.x * 2;
    }

    /// <summary>
    /// Parses string to Int *fast*
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ParseToInt(this string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
    }

    /// <summary>
    /// Returns mouse positions that hits the specified _layerMask
    /// </summary>
    /// <param name="_layerMask"></param>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPosition(LayerMask _layerMask)
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray,out RaycastHit raycastHit, 999f, _layerMask))
		{
            return raycastHit.point;
		}else
		{
            return Vector3.zero;
		}

    }
    /// <summary>
    /// Fastly change X of a Vector3
    /// </summary>
    /// <param name="_curVector"></param>
    /// <param name="_newX"></param>
    /// <returns></returns>
    public static Vector3 ChangeX(this Vector3 _curVector,float _newX)
	{
        _curVector.x = _newX;
        return _curVector;
	}

    /// <summary>
    /// Fastly change Y of a Vector3
    /// </summary>
    /// <param name="_curVector"></param>
    /// <param name="_newY"></param>
    /// <returns></returns>
    public static Vector3 ChangeY(this Vector3 _curVector, float _newY)
    {
        _curVector.y = _newY;
        return _curVector;
    }

    /// <summary>
    /// Fastly change Z of a Vector3
    /// </summary>
    /// <param name="_curVector"></param>
    /// <param name="_newZ"></param>
    /// <returns></returns>
    public static Vector3 ChangeZ(this Vector3 _curVector, float _newZ)
    {
        _curVector.z = _newZ;
        return _curVector;
    }

	/// <summary>
	/// Fastly change Z of a Vector3
	/// </summary>
	/// <param name="_curVector"></param>
	/// <param name="_newZ"></param>
	/// <returns></returns>
	public static Vector2 ChangeY2D(this Vector2 _curVector, float _newZ)
	{
		_curVector.y = _newZ;
		return _curVector;
	}

	/// <summary>
	/// Returns a negative number if B is left of A, positive if right of A, or 0 if they are perfectly aligned
	/// </summary>
	/// <param name="A"></param>
	/// <param name="B"></param>
	/// <returns></returns>
	public static float AngleDir(Vector2 A, Vector2 B)
	{
		return -A.x * B.y + A.y * B.x;
	}

}
