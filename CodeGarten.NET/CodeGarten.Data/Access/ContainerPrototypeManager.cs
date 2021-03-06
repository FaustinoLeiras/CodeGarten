﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeGarten.Data.Model;
using CodeGarten.Data.ModelView;

namespace CodeGarten.Data.Access
{
    public sealed class ContainerPrototypeManager
    {
        private readonly Context _dbContext;

        public ContainerPrototypeManager(DataBaseManager db)
        {
            _dbContext = db.DbContext;
        }

        public void Create(ContainerPrototypeView containerPrototypeView, long structure)
        {
            if (containerPrototypeView == null) throw new ArgumentNullException("containerPrototypeView");

            var containerPrototype = containerPrototypeView.Convert();

            var structureObj = StructureManager.Get(_dbContext, structure);
            if (structureObj == null) throw new ArgumentException("\"structure\" is a invalid argument");

            containerPrototype.Structure = structureObj;

            _dbContext.ContainerPrototypes.Add(containerPrototype);

            _dbContext.SaveChanges();
        }

        public void Create(ContainerPrototypeView containerPrototypeView, long structure,
                           string parentContainerPrototype)
        {
            if (containerPrototypeView == null) throw new ArgumentNullException("containerPrototypeView");

            var containerPrototype = containerPrototypeView.Convert();

            var structureObj = StructureManager.Get(_dbContext, structure);
            if (structureObj == null) throw new ArgumentException("\"structure\" is a invalid argument");

            var parentContainerPrototypeObj = Get(_dbContext, structure, parentContainerPrototype);
            if (parentContainerPrototypeObj == null)
                throw new ArgumentException("\"parentContainerPrototype\" is a invalid argument");

            containerPrototype.Parent = parentContainerPrototypeObj;

            containerPrototype.Structure = structureObj;

            _dbContext.ContainerPrototypes.Add(containerPrototype);

            _dbContext.SaveChanges();
        }

        public void Delete(ContainerPrototypeView containerPrototypeView, long structureId)
        {
            //_dbContext.Entry(_dbContext.ContainerPrototypes.Find(containerPrototypeView.Name, structureId)).State = EntityState.Deleted;
            Delete(containerPrototypeView.Name, structureId);
            
        }

        public void Delete(string containerPrototype, long structureId)
        {
            var containerPrototypeObj = _dbContext.ContainerPrototypes.Find(containerPrototype, structureId);
            if(containerPrototypeObj == null)
                throw new Exception();

            _dbContext.ContainerPrototypes.Remove(containerPrototypeObj);

            _dbContext.SaveChanges();
        }

        internal static ContainerPrototype Get(Context db, long structure, string containerPrototype)
        {
            return db.ContainerPrototypes.Where(
                (cp) =>
                cp.Name == containerPrototype &&
                cp.StructureId == structure
                ).SingleOrDefault();
        }

        public ContainerPrototypeView Get(long structure, string containerPrototype)
        {
            var containerProto = Get(_dbContext, structure, containerPrototype);

            return containerProto == null ? null : containerProto.Convert();
        }
        //TODO
        public IEnumerable<ContainerPrototype> GetAll(long structureId)
        {
            return _dbContext.ContainerPrototypes.Where(cp => cp.StructureId == structureId);
        }

        public bool AddWorkSpaceType(long structure, string containerPrototype, string workspaceType)
        {
            var workspaceTypeObj = WorkSpaceTypeManager.Get(_dbContext, structure, workspaceType);
            if (workspaceTypeObj == null)
                throw new ArgumentException("\"workspaceType\" or \"structure\" is a invalid argument");

            var containerPrototypeObj = Get(_dbContext, structure, containerPrototype);
            if (containerPrototypeObj == null)
                throw new ArgumentException("\"containerPrototype\" or \"structure\" is a invalid argument");

            if (containerPrototypeObj.WorkSpaceTypes.Where(
                (wk) =>
                wk.Name == workspaceType &&
                wk.StructureId == structure
                    ).Count() != 0) return false;

            containerPrototypeObj.WorkSpaceTypes.Add(workspaceTypeObj);

            _dbContext.Entry(containerPrototypeObj).State = EntityState.Modified;

            return _dbContext.SaveChanges() != 0;
        }

        public bool RemoveWorkSpaceType(long structure, string containerPrototype, string workspaceType)
        {
            var workspaceTypeObj = WorkSpaceTypeManager.Get(_dbContext, structure, workspaceType);
            if (workspaceTypeObj == null)
                throw new ArgumentException("\"workspaceType\" or \"structure\" is a invalid argument");

            var containerPrototypeObj = Get(_dbContext, structure, containerPrototype);
            if (containerPrototypeObj == null)
                throw new ArgumentException("\"containerPrototype\" or \"structure\" is a invalid argument");

            containerPrototypeObj.WorkSpaceTypes.Remove(workspaceTypeObj);

            _dbContext.Entry(containerPrototypeObj).State = EntityState.Modified;

            return _dbContext.SaveChanges() != 0;
        }

        public bool AddChild(long structure, string containerPrototype, string childContainerPrototype)
        {
            var childContainerPrototypeObj = Get(_dbContext, structure, childContainerPrototype);
            if (childContainerPrototypeObj == null)
                throw new ArgumentException("\"childContainerPrototype\" or \"structure\" is a invalid argument");

            var containerPrototypeObj = Get(_dbContext, structure, containerPrototype);
            if (containerPrototypeObj == null)
                throw new ArgumentException("\"containerPrototype\" or \"structure\" is a invalid argument");

            if (containerPrototypeObj.Childs.Where(
                (c) =>
                c.Name == containerPrototype
                    ).Count() != 0) return false;

            containerPrototypeObj.Childs.Add(childContainerPrototypeObj);

            _dbContext.Entry(containerPrototypeObj).State = EntityState.Modified;

            return _dbContext.SaveChanges() != 0;
        }
    }
}