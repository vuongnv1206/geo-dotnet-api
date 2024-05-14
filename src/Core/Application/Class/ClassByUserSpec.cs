﻿using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class ClassByUserSpec : Specification<Classes, ClassDto>, ISingleResultSpecification
{
    public ClassByUserSpec(Guid userId)
    {
        Query.Where(b => b.OwnerId == userId);
    }
}
