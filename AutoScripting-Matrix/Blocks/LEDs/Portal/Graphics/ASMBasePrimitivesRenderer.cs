using Engine.Media;
using System;
using System.Collections.Generic;

namespace Engine.Graphics
{
	public class ASMBasePrimitivesRenderer<T1, T2, T3> where T1 : BaseFlatBatch, new() where T2 : BaseTexturedBatch, new() where T3 : BaseFontBatch, new()
	{
		public bool m_sortNeeded;

		public List<BaseBatch> m_allBatches = [];

		public LinkedList<T1> m_flatBatches = new();

		public LinkedList<T2> m_texturedBatches = new();

		public LinkedList<T3> m_fontBatches = new();

        public ASMBasePrimitivesRenderer()
		{
		}

        public T1 FindFlatBatch(int layer, DepthStencilState depthStencilState, RasterizerState rasterizerState, BlendState blendState)
		{
			for (LinkedListNode<T1> linkedListNode = m_flatBatches.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				T1 value = linkedListNode.Value;
				if (layer == value.Layer && depthStencilState == value.DepthStencilState && rasterizerState == value.RasterizerState && blendState == value.BlendState)
				{
					if (linkedListNode.Previous != null)
					{
						m_flatBatches.Remove(linkedListNode);
						m_flatBatches.AddFirst(linkedListNode);
					}
					return value;
				}
			}
			m_sortNeeded |= m_allBatches.Count > 0 && m_allBatches[^1].Layer > layer;
			var val = new T1();
			val.Layer = layer;
			val.DepthStencilState = depthStencilState;
			val.RasterizerState = rasterizerState;
			val.BlendState = blendState;
			m_flatBatches.AddFirst(val);
			m_allBatches.Add(val);
			return val;
		}

        public T2 FindTexturedBatch(Texture2D texture, bool useAlphaTest, int layer, DepthStencilState depthStencilState, RasterizerState rasterizerState, BlendState blendState, SamplerState samplerState)
		{
			ArgumentNullException.ThrowIfNull(texture);
			for (LinkedListNode<T2> linkedListNode = m_texturedBatches.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				T2 value = linkedListNode.Value;
				if (texture == value.Texture && useAlphaTest == value.UseAlphaTest && layer == value.Layer && depthStencilState == value.DepthStencilState && rasterizerState == value.RasterizerState && blendState == value.BlendState && samplerState == value.SamplerState)
				{
					if (linkedListNode.Previous != null)
					{
						m_texturedBatches.Remove(linkedListNode);
						m_texturedBatches.AddFirst(linkedListNode);
					}
					return value;
				}
			}
			m_sortNeeded |= m_allBatches.Count > 0 && m_allBatches[^1].Layer > layer;
			var val = new T2();
			val.Layer = layer;
			val.UseAlphaTest = useAlphaTest;
			val.Texture = texture;
			val.SamplerState = samplerState;
			val.DepthStencilState = depthStencilState;
			val.RasterizerState = rasterizerState;
			val.BlendState = blendState;
			m_texturedBatches.AddFirst(val);
			m_allBatches.Add(val);
			return val;
		}

        public T3 FindFontBatch(BitmapFont font, int layer, DepthStencilState depthStencilState, RasterizerState rasterizerState, BlendState blendState, SamplerState samplerState)
		{
			ArgumentNullException.ThrowIfNull(font);
			for (LinkedListNode<T3> linkedListNode = m_fontBatches.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				T3 value = linkedListNode.Value;
				if (font == value.Font && layer == value.Layer && depthStencilState == value.DepthStencilState && rasterizerState == value.RasterizerState && blendState == value.BlendState && samplerState == value.SamplerState)
				{
					if (linkedListNode.Previous != null)
					{
						m_fontBatches.Remove(linkedListNode);
						m_fontBatches.AddFirst(linkedListNode);
					}
					return value;
				}
			}
			m_sortNeeded |= m_allBatches.Count > 0 && m_allBatches[^1].Layer > layer;
			var val = new T3();
			val.Layer = layer;
			val.Font = font;
			val.SamplerState = samplerState;
			val.DepthStencilState = depthStencilState;
			val.RasterizerState = rasterizerState;
			val.BlendState = blendState;
			m_fontBatches.AddFirst(val);
			m_allBatches.Add(val);
			return val;
		}

		public void Flush(Matrix matrix, bool clearAfterFlush = true, int maxLayer = int.MaxValue)
		{
			if (m_sortNeeded)
			{
				m_sortNeeded = false;
				m_allBatches.Sort(delegate (BaseBatch b1, BaseBatch b2)
				{
					if (b1.Layer < b2.Layer)
					{
						return -1;
					}
					return (b1.Layer > b2.Layer) ? 1 : 0;
				});
			}
			foreach (BaseBatch allBatch in m_allBatches)
			{
				if (allBatch.Layer > maxLayer)
				{
					break;
				}
				if (!allBatch.IsEmpty())
				{
					allBatch.Flush(matrix, clearAfterFlush);
				}
			}
		}

		public void Clear()
		{
			foreach (BaseBatch allBatch in m_allBatches)
			{
				allBatch.Clear();
			}
		}
	}
}
