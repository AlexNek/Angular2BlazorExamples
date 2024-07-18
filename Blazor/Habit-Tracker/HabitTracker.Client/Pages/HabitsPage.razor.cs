using HabitTracker.Client.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace HabitTracker.Client.Pages
{
    public partial class HabitsPage : ComponentBase
    {
        private const string DefDaily = "Daily";

        private const string DefMonthly = "Monthly";

        private const string DefWeekly = "Weekly";


        //private string? _description;

        //private string? _frequency= DefDaily;

        private readonly List<Habit> _habits = new List<Habit>
                                                   {
                                                       new Habit()
                                                           {
                                                               Name = "15 Minute Walk",
                                                               Frequency = "Daily",
                                                               Description =
                                                                   "Getting outside and enjoying the fresh air helps me clear my head and improves my mood."
                                                           },
                                                       new Habit()
                                                           {
                                                               Name = "Weed the Garden",
                                                               Frequency = "Weekly",
                                                               Description =
                                                                   "The weeds get so out of hand if they wait any longer, and I like how nice our home looks with a clean lawn."
                                                           },
                                                   };

        //private string? _name;

        private bool _adding;

        private bool _editing;

        private Habit? _editedHabit;

        //private int editingIndex = 0;

        //private Validations? validationsBasicExampleRef;

        private Validations? validationsFormRef;

        public IReadOnlyList<Habit> Habits
        {
            get
            {
                return _habits;
            }
        }

        private void CancelForm(MouseEventArgs obj)
        {
            ExitForm();
        }

        private void ExitForm()
        {
            _adding = false;
            _editing = false;
            //this.habitForm.reset();
        }

        private void OnNewHabitClicked()
        {
            _adding = true;
            _editedHabit = new Habit();
            _editedHabit.Frequency = DefDaily;
            _habits.Add(_editedHabit);
        }

        private async Task SubmitForm()
        {
            bool validateAll = await validationsFormRef!.ValidateAll();
            if (validateAll)
            {
                await validationsFormRef.ClearAll();
                int foundIndex = _habits.FindIndex(item => _editedHabit != null && _editedHabit.Id == item.Id);
                if (foundIndex >= 0)
                {
                    Habit foundHabit = _habits[foundIndex];
                    if (_editedHabit != null)
                    {
                        foundHabit.Name = _editedHabit.Name;
                        foundHabit.Frequency = _editedHabit.Frequency;
                        foundHabit.Description = _editedHabit.Description;
                    }
                }

                _editing = false;
                _adding = false;
            }
        }

        private void ValidateEmptyString(ValidatorEventArgs e)
        {
            string? value = (string?)e.Value;
            e.Status = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? ValidationStatus.Error : ValidationStatus.Success;
        }

        public void OnDelete(Habit? habit)
        {
            if (habit != null)
            {
                _habits.Remove(habit);
            }
        }

        //public object? SetEditForm( Habit habit, int index)
        //{
        //    //this.habitForm.patchValue({
        //    //    name: habit.name,
        //    //    frequency: habit.frequency,
        //    //    description: habit.description,
        //    //});
        //    this.editing = true;
        //    this.editingIndex = index;
        //    return null;
        //}

        //private Habit? _currentHabit;

        //private object SetCurrentHabit(int habitIndex)
        //{
        //    _currentHabit = Habits[habitIndex];
        //    return null!;
        //}

        private void SetEditForm(Habit? habit)
        {
            if (habit != null)
            {
                _editedHabit = habit.Clone();
                _editing = true;
            }
        }
    }
}
