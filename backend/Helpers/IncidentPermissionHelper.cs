using LogisticsTroubleManagement.Models;

namespace LogisticsTroubleManagement.Helpers
{
    /// <summary>
    /// インシデント操作権限チェックヘルパークラス
    /// </summary>
    public static class IncidentPermissionHelper
    {
        // ロールID定数
        private const int SystemAdminRoleId = 1;
        private const int OfficeAdminRoleId = 2;
        private const int GeneralOfficeRoleId = 3;
        private const int ThreePLRoleId = 4;

        // ステータス定数
        private const string SecondInfoInvestigation = "2次情報調査中";
        private const string SecondInfoDelayed = "2次情報遅延";
        private const string ThirdInfoInvestigation = "3次情報調査中";
        private const string ThirdInfoDelayed = "3次情報遅延";
        private const string Completed = "完了";

        /// <summary>
        /// 1次情報登録（新規登録）が可能かどうかを判定
        /// </summary>
        /// <param name="userRoleId">ユーザーロールID</param>
        /// <returns>可能な場合true</returns>
        public static bool CanCreateFirstInfo(int userRoleId)
        {
            // システム管理者、事務管理者、一般事務は1次情報登録可能
            return userRoleId == SystemAdminRoleId ||
                   userRoleId == OfficeAdminRoleId ||
                   userRoleId == GeneralOfficeRoleId;
        }

        /// <summary>
        /// 1次情報修正が可能かどうかを判定
        /// </summary>
        /// <param name="userRoleId">ユーザーロールID</param>
        /// <param name="status">現在のステータス</param>
        /// <param name="incident">インシデント（N+1情報登録後の制御用）</param>
        /// <returns>可能な場合true</returns>
        public static bool CanUpdateFirstInfo(int userRoleId, string status, Incident? incident = null)
        {
            // システム管理者と事務管理者は常に可能
            if (userRoleId == SystemAdminRoleId || userRoleId == OfficeAdminRoleId)
            {
                return true;
            }

            // 一般事務は1次情報修正可能
            if (userRoleId == GeneralOfficeRoleId)
            {
                // ステータスチェック：2次情報調査中または2次情報遅延の状態のみ
                if (status != SecondInfoInvestigation && status != SecondInfoDelayed)
                {
                    return false;
                }

                // N+1情報登録後の制御：2次情報が登録されている場合は修正不可
                if (incident != null && incident.InputDate.HasValue)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 2次情報登録が可能かどうかを判定
        /// </summary>
        /// <param name="userRoleId">ユーザーロールID</param>
        /// <param name="status">現在のステータス</param>
        /// <param name="incident">インシデント</param>
        /// <returns>可能な場合true</returns>
        public static bool CanCreateSecondInfo(int userRoleId, string status, Incident? incident = null)
        {
            // システム管理者、事務管理者、3PLは2次情報登録可能
            if (userRoleId == SystemAdminRoleId ||
                userRoleId == OfficeAdminRoleId ||
                userRoleId == ThreePLRoleId)
            {
                // ステータスチェック：2次情報調査中または2次情報遅延の状態のみ
                if (status != SecondInfoInvestigation && status != SecondInfoDelayed)
                {
                    return false;
                }

                // 既に2次情報が登録されている場合は登録不可（修正として扱う）
                if (incident != null && incident.InputDate.HasValue)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 2次情報修正が可能かどうかを判定
        /// </summary>
        /// <param name="userRoleId">ユーザーロールID</param>
        /// <param name="status">現在のステータス</param>
        /// <param name="incident">インシデント（N+1情報登録後の制御用）</param>
        /// <returns>可能な場合true</returns>
        public static bool CanUpdateSecondInfo(int userRoleId, string status, Incident? incident = null)
        {
            // システム管理者と事務管理者は常に可能
            if (userRoleId == SystemAdminRoleId || userRoleId == OfficeAdminRoleId)
            {
                return true;
            }

            // 3PLは2次情報修正可能
            if (userRoleId == ThreePLRoleId)
            {
                // ステータスチェック：3次情報調査中または3次情報遅延の状態のみ
                if (status != ThirdInfoInvestigation && status != ThirdInfoDelayed)
                {
                    return false;
                }

                // N+1情報登録後の制御：3次情報が登録されている場合は修正不可
                if (incident != null && incident.InputDate3.HasValue)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 3次情報登録が可能かどうかを判定
        /// </summary>
        /// <param name="userRoleId">ユーザーロールID</param>
        /// <param name="status">現在のステータス</param>
        /// <param name="incident">インシデント</param>
        /// <returns>可能な場合true</returns>
        public static bool CanCreateThirdInfo(int userRoleId, string status, Incident? incident = null)
        {
            // システム管理者、事務管理者、3PLは3次情報登録可能
            if (userRoleId == SystemAdminRoleId ||
                userRoleId == OfficeAdminRoleId ||
                userRoleId == ThreePLRoleId)
            {
                // ステータスチェック：3次情報調査中または3次情報遅延の状態のみ
                if (status != ThirdInfoInvestigation && status != ThirdInfoDelayed)
                {
                    return false;
                }

                // 既に3次情報が登録されている場合は登録不可（修正として扱う）
                if (incident != null && incident.InputDate3.HasValue)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 3次情報修正が可能かどうかを判定
        /// </summary>
        /// <param name="userRoleId">ユーザーロールID</param>
        /// <param name="status">現在のステータス</param>
        /// <param name="incident">インシデント</param>
        /// <returns>可能な場合true</returns>
        public static bool CanUpdateThirdInfo(int userRoleId, string status, Incident? incident = null)
        {
            // システム管理者、事務管理者、3PLは3次情報修正可能
            if (userRoleId == SystemAdminRoleId ||
                userRoleId == OfficeAdminRoleId ||
                userRoleId == ThreePLRoleId)
            {
                // 3次情報が登録済み（InputDate3が設定されている）場合のみ修正可能
                if (incident != null && incident.InputDate3.HasValue)
                {
                    // ステータスが「完了」の場合、または「3次情報調査中」「3次情報遅延」で再発防止策が空の場合も編集可能
                    if (status == Completed)
                    {
                        return true;
                    }
                    // 再発防止策が空またはnullの場合（未完了状態）も編集可能
                    if ((status == ThirdInfoInvestigation || status == ThirdInfoDelayed) &&
                        string.IsNullOrEmpty(incident.RecurrencePreventionMeasures))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// 更新DTOから更新対象の情報段階を判定
        /// </summary>
        /// <param name="updateDto">更新DTO</param>
        /// <param name="incident">既存のインシデント</param>
        /// <returns>更新対象の情報段階（1, 2, 3、または複数）</returns>
        public static List<int> GetUpdatedInfoLevels(DTOs.UpdateIncidentDto updateDto, Incident incident)
        {
            var levels = new List<int>();

            // 1次情報の更新チェック
            bool isFirstInfoUpdated = updateDto.CreationDate.HasValue ||
                                      updateDto.Organization.HasValue ||
                                      !string.IsNullOrEmpty(updateDto.Creator) ||
                                      updateDto.OccurrenceDateTime.HasValue ||
                                      updateDto.OccurrenceLocation.HasValue ||
                                      updateDto.ShippingWarehouse.HasValue ||
                                      updateDto.ShippingCompany.HasValue ||
                                      updateDto.TroubleCategory.HasValue ||
                                      updateDto.TroubleDetailCategory.HasValue ||
                                      !string.IsNullOrEmpty(updateDto.Details) ||
                                      updateDto.VoucherNumber != null ||
                                      updateDto.CustomerCode != null ||
                                      updateDto.ProductCode != null ||
                                      updateDto.Quantity.HasValue ||
                                      updateDto.Unit.HasValue;

            // 2次情報の更新チェック
            bool isSecondInfoUpdated = updateDto.InputDate.HasValue ||
                                       !string.IsNullOrEmpty(updateDto.ProcessDescription) ||
                                       !string.IsNullOrEmpty(updateDto.Cause) ||
                                       !string.IsNullOrEmpty(updateDto.PhotoDataUri);

            // 3次情報の更新チェック
            bool isThirdInfoUpdated = updateDto.InputDate3.HasValue ||
                                      !string.IsNullOrEmpty(updateDto.RecurrencePreventionMeasures);

            // 2次情報登録の判定（既存がnullで新規に設定される場合）
            if (isSecondInfoUpdated && !incident.InputDate.HasValue && updateDto.InputDate.HasValue)
            {
                levels.Add(2);
            }
            // 2次情報修正の判定（既存が設定済みで更新される場合）
            else if (isSecondInfoUpdated && incident.InputDate.HasValue)
            {
                levels.Add(2);
            }

            // 3次情報登録の判定（既存がnullで新規に設定される場合）
            if (isThirdInfoUpdated && !incident.InputDate3.HasValue && updateDto.InputDate3.HasValue)
            {
                levels.Add(3);
            }
            // 3次情報修正の判定（既存が設定済みで更新される場合）
            else if (isThirdInfoUpdated && incident.InputDate3.HasValue)
            {
                levels.Add(3);
            }

            // 1次情報修正の判定（2次情報・3次情報の更新がない場合、または1次情報のみ更新される場合）
            if (isFirstInfoUpdated && !isSecondInfoUpdated && !isThirdInfoUpdated)
            {
                levels.Add(1);
            }
            // 1次情報と2次情報・3次情報が同時に更新される場合は、1次情報修正として扱う
            else if (isFirstInfoUpdated && (isSecondInfoUpdated || isThirdInfoUpdated))
            {
                // 1次情報修正は別途チェックが必要な場合があるため、ここでは追加しない
                // 実際の権限チェックでは、1次情報フィールドの更新を個別にチェック
            }

            return levels;
        }
    }
}

