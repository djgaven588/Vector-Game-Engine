using System;
using System.Collections.Generic;

namespace VectorEngine.Engine.Common
{
    public class LayerManager
    {
        private List<GameLayer> normalLayers = new List<GameLayer>();
        private List<GameLayer> overlayLayers = new List<GameLayer>();

        public void AddLayer(GameLayer layer, bool prioritize)
        {
            if (prioritize)
            {
                normalLayers.Insert(0, layer);
            }
            else
            {
                normalLayers.Add(layer);
            }
        }

        public void AddOverlayLayer(GameLayer layer, bool prioritize)
        {
            if (prioritize)
            {
                overlayLayers.Insert(0, layer);
            }
            else
            {
                overlayLayers.Add(layer);
            }
        }

        public void RemoveLayer(GameLayer layer)
        {
            normalLayers.Remove(layer);
        }

        public void RemoveOverlayLayer(GameLayer layer)
        {
            overlayLayers.Remove(layer);
        }

        public void RunLayerUpdate(double deltaTime)
        {
            for (int i = normalLayers.Count - 1; i >= 0; i--)
            {
                try
                {
                    normalLayers[i].OnUpdate(deltaTime);
                }
                catch(Exception e)
                {
                    Debug.Log($"ERROR During Layer Update: {e.ToString()}");
                }
            }

            for (int i = overlayLayers.Count - 1; i >= 0; i--)
            {
                try
                {
                    overlayLayers[i].OnUpdate(deltaTime);
                }
                catch (Exception e)
                {
                    Debug.Log($"ERROR During Overlay Layer Update: {e.ToString()}");
                }
            }
        }

        public void RunLayerRender(double deltaTime)
        {
            for (int i = normalLayers.Count - 1; i >= 0; i--)
            {
                try
                {
                    normalLayers[i].OnRender(deltaTime);
                }
                catch (Exception e)
                {
                    Debug.Log($"ERROR During Layer Render: {e.ToString()}");
                }
            }

            for (int i = overlayLayers.Count - 1; i >= 0; i--)
            {
                try
                {
                    overlayLayers[i].OnRender(deltaTime);
                }
                catch (Exception e)
                {
                    Debug.Log($"ERROR During Overlay Layer Render: {e.ToString()}");
                }
            }
        }

        public void RunLayerEvent()
        {
            bool eventEaten = false;
            for (int i = 0; i < overlayLayers.Count; i++)
            {
                try
                {
                    overlayLayers[i].OnEvent(ref eventEaten);
                    if (eventEaten)
                        return;
                }
                catch (Exception e)
                {
                    Debug.Log($"ERROR During Overlay Layer Event: {e.ToString()}");
                }
            }

            for (int i = 0; i < overlayLayers.Count; i++)
            {
                try
                {
                    normalLayers[i].OnEvent(ref eventEaten);
                    if (eventEaten)
                        return;
                }
                catch (Exception e)
                {
                    Debug.Log($"ERROR During Layer Event: {e.ToString()}");
                }
            }
        }
    }
}
