using System;

namespace Virtual.Domain.Models
{
    public class PacienteConvenio : Entity
    {
        public Guid PacienteId { get; set; }
        public Guid ConvenioId { get; set; }

        /* EF Relations */
        public Paciente Paciente { get; set; }
        public Convenio Convenio { get; set; }
    }
}
