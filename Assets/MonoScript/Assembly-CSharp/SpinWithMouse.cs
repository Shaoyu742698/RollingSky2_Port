using UnityEngine;

public class SpinWithMouse : MonoBehaviour
{
	public Transform target;

	public float speed = 1f;

	private Transform mTrans;

	private void Start()
	{
		mTrans = base.transform;
	}

	private void OnDrag(Vector2 delta)
	{
		if (target != null)
		{
			target.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * speed, 0f) * target.localRotation;
		}
		else
		{
			mTrans.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * speed, 0f) * mTrans.localRotation;
		}
	}
}
