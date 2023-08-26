using System.IO;

namespace Umeng
{
	public class JSONNumber : JSONNode
	{
		private double m_Data;

		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		public override string Value
		{
			get
			{
				return m_Data.ToString();
			}
			set
			{
				double result;
				if (double.TryParse(value, out result))
				{
					m_Data = result;
				}
			}
		}

		public override double AsDouble
		{
			get
			{
				return m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public JSONNumber(double aData)
		{
			m_Data = aData;
		}

		public JSONNumber(string aData)
		{
			Value = aData;
		}

		public override string ToString()
		{
			return m_Data.ToString();
		}

		internal override string ToString(string aIndent, string aPrefix)
		{
			return m_Data.ToString();
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write((byte)4);
			aWriter.Write(m_Data);
		}
	}
}
