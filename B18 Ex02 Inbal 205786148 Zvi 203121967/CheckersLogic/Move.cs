using System;
using System.Collections.Generic;
using System.Text;

namespace CheckersLogic
{
    public struct Move
    {
        internal int m_SourceRow;
        internal int m_SourceCol;
        internal int m_DestinationRow;
        internal int m_DestinationCol;

        public Move(int i_SourceRow, int i_SourceCol, int i_DestinationRow, int i_DestinationCol)
        {
            m_SourceRow = i_SourceRow;
            m_SourceCol = i_SourceCol;
            m_DestinationRow = i_DestinationRow;
            m_DestinationCol = i_DestinationCol;
        }

        public bool isJump()
        {
            int delta = m_DestinationRow - m_SourceRow;
            return delta == 2 || delta == -2;
        }
    }
}
