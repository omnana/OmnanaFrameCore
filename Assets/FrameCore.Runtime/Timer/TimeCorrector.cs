namespace FrameCore.Runtime
{
    public class TimeCorrector
    {
        private float _updateTime;
        private float _leftTime;
        private float _addedLeftTime;
        private float _lastTime;

        public TimeCorrector()
        {
            _updateTime = 0;
            _leftTime = 0;
            _addedLeftTime = 0;
            _lastTime = UnityEngine.Time.time;
        }

        public int GetCorrectedTime()
        {
            var current = UnityEngine.Time.time;
            var deltaTime = current - _lastTime;
            _lastTime = current;
            _updateTime = deltaTime;
            var elapse = (int) (_updateTime * 1000);
            _leftTime = _updateTime - elapse / 1000.0f;
            _addedLeftTime += _leftTime;

            if (_addedLeftTime >= 0.001)
            {
                elapse += 1;
                _addedLeftTime -= 0.001f;
            }

            return elapse;
        }
    }
}
