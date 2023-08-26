using System;
using System.IO;
using Foundation;
using UnityEngine;

namespace RS2
{
	public class NormalTileAsFragement : BaseTile
	{
		[Serializable]
		public struct NormalTileAsFragementData : IReadWriteBytes
		{
			public float m_beginDistance;

			public float m_resetDistance;

			[Header("GreaterThanOrEqual(大于等于) LessThanOrEqual(小于等于) GreaterThan(大于) Equal（等于） LessThan（小于）")]
			public OperatorType m_operatorType;

			public int m_needFragmentCount;

			public void ReadBytes(byte[] bytes)
			{
				int startIndex = 0;
				m_beginDistance = bytes.GetSingle(ref startIndex);
				m_resetDistance = bytes.GetSingle(ref startIndex);
				m_operatorType = (OperatorType)bytes.GetInt32(ref startIndex);
				m_needFragmentCount = bytes.GetInt32(ref startIndex);
			}

			public byte[] WriteBytes()
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					int offset = 0;
					memoryStream.WriteByteArray(m_beginDistance.GetBytes(), ref offset);
					memoryStream.WriteByteArray(m_resetDistance.GetBytes(), ref offset);
					memoryStream.WriteByteArray(((int)m_operatorType).GetBytes(), ref offset);
					memoryStream.WriteByteArray(m_needFragmentCount.GetBytes(), ref offset);
					memoryStream.Flush();
					memoryStream.Seek(0L, SeekOrigin.Begin);
					return memoryStream.ToArray();
				}
			}
		}

		public NormalTileAsFragementData m_data;

		private int m_currentFragment;

		private Renderer[] m_renderers;

		public override bool IfRebirthRecord
		{
			get
			{
				return false;
			}
		}

		public override void LateInitialize()
		{
			base.LateInitialize();
			m_renderers = base.gameObject.GetComponentsInChildren<Renderer>();
			SetRenderer(m_renderers, false);
			commonState = CommonState.None;
			m_currentFragment = 0;
		}

		public override void UpdateElement()
		{
			float num = base.groupTransform.InverseTransformPoint(BaseRole.BallPosition).z - base.groupTransform.InverseTransformPoint(base.transform.position).z;
			if (commonState == CommonState.None)
			{
				if (num >= m_data.m_beginDistance)
				{
					OnTriggerPlay();
					commonState = CommonState.Active;
				}
			}
			else if (commonState == CommonState.Active && num >= m_data.m_resetDistance)
			{
				OnTriggerStop();
				commonState = CommonState.End;
			}
		}

		public override void ResetElement()
		{
			base.ResetElement();
			commonState = CommonState.None;
			StopListeningForCollectionEvents();
		}

		public override void OnTriggerPlay()
		{
			base.OnTriggerPlay();
			Mod.Event.Subscribe(EventArgs<GainedDropEventArgs>.EventId, OnCollectEventCall);
		}

		public override void OnTriggerStop()
		{
			base.OnTriggerStop();
			SetRenderer(m_renderers, false);
			StopListeningForCollectionEvents();
		}

		private void OnCollectEventCall(object sender, Foundation.EventArgs e)
		{
			GainedDropEventArgs gainedDropEventArgs = e as GainedDropEventArgs;
			if (gainedDropEventArgs != null && gainedDropEventArgs.m_dropData.m_type == DropType.TRIGGERFRAGMENT)
			{
				DealCollectEventCallbacks();
			}
		}

		private void DealCollectEventCallbacks()
		{
			m_currentFragment++;
			if (commonState == CommonState.Active)
			{
				bool flag = false;
				switch (m_data.m_operatorType)
				{
				case OperatorType.GreaterThanOrEqual:
					flag = m_currentFragment >= m_data.m_needFragmentCount;
					break;
				case OperatorType.LessThanOrEqual:
					flag = m_currentFragment <= m_data.m_needFragmentCount;
					break;
				case OperatorType.GreaterThan:
					flag = m_currentFragment > m_data.m_needFragmentCount;
					break;
				case OperatorType.Equal:
					flag = m_currentFragment == m_data.m_needFragmentCount;
					break;
				case OperatorType.LessThan:
					flag = m_currentFragment < m_data.m_needFragmentCount;
					break;
				}
				if (flag)
				{
					SetRenderer(m_renderers, true);
					StopListeningForCollectionEvents();
				}
			}
		}

		private void StopListeningForCollectionEvents()
		{
			Mod.Event.Unsubscribe(EventArgs<GainedDropEventArgs>.EventId, OnCollectEventCall);
			m_currentFragment = 0;
			commonState = CommonState.End;
		}

		public override string Write()
		{
			return JsonUtility.ToJson(m_data);
		}

		public override void Read(string info)
		{
			m_data = JsonUtility.FromJson<NormalTileAsFragementData>(info);
		}

		public override void ReadBytes(byte[] bytes)
		{
			m_data = StructTranslatorUtility.ToStructure<NormalTileAsFragementData>(bytes);
		}

		public override byte[] WriteBytes()
		{
			return StructTranslatorUtility.ToByteArray(m_data);
		}

		public override void SetDefaultValue(object[] objs)
		{
			m_data = (NormalTileAsFragementData)objs[0];
		}

		public void OnDrawGizmos()
		{
			Vector3 position = base.gameObject.transform.position;
			Vector3 from = base.gameObject.transform.TransformPoint(new Vector3(0f, 0f, m_data.m_beginDistance));
			Gizmos.color = Color.red;
			Gizmos.DrawLine(from, position);
			Gizmos.color = Color.white;
		}

		private void SetRenderer(Renderer[] renderers, bool enabled)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].enabled = enabled;
			}
		}
	}
}
