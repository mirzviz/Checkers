namespace CheckersGameProject
{
    public class CheckersPlayer
    {
        private string m_Name;
        private int m_Points = 0;
      
        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_Name = value;
            }
        }
        
        public int Points
        {
            get
            {
                return m_Points;
            }

            set
            {
                m_Points = value;
            }
        }
    }
}
