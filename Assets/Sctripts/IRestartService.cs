    public interface IRestartService : IService
    {
        public void Restart();

        public void Subscribe(IRestartable restartable);
        
        public void Unsubscribe(IRestartable restartable);
    }
