using Factory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Factory.Controllers
{
  public class EngineersController : Controller
  {
    private readonly FactoryContext _db;

    public EngineersController(FactoryContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Engineer> model = _db.Engineers.ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Engineer engineer)
    {
      _db.Engineers.Add(engineer);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var model = _db.Engineers
        .Include(engineer => engineer.JoinEntities)
        .ThenInclude(join => join.Machine)
        .FirstOrDefault(engineer => engineer.EngineerId == id);
      return View(model);
    }

    public ActionResult AddMachine(int id)
    {
      var model = _db.Engineers
        .Include(engineer => engineer.JoinEntities)
        .ThenInclude(join => join.Machine)
        .FirstOrDefault(engineer => engineer.EngineerId == id);
      ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "MachineName");
      return View(model);
    }

    [HttpPost]
    public ActionResult AddMachine(Engineer engineer, int MachineId)
    {
      if (MachineId != 0 && !_db.EngineerMachines.Any(model => model.EngineerId == engineer.EngineerId && model.MachineId == MachineId))
      {
        _db.EngineerMachines.Add(new EngineerMachines() { MachineId = MachineId, EngineerId = engineer.EngineerId });
        _db.SaveChanges();
      }
      return RedirectToAction("AddMachine");
    }

    public ActionResult Edit(int id)
    {
      var model = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
      return View(model);
    }

    [HttpPost]
    public ActionResult Edit(Engineer engineer)
    {
      _db.Entry(engineer).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteMachine(int joinId, int id)
    {
      var joinEntry = _db.EngineerMachines.FirstOrDefault(entry => entry.EngineerMachinesId == joinId);
      _db.EngineerMachines.Remove(joinEntry);
      _db.SaveChanges();
      var model = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
      return View("Details", model);
    }
    public ActionResult Delete(int id)
    {
      var model = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
      return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var model = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
      _db.Engineers.Remove(model);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}